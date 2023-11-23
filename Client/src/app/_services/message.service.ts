import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { getPaginationHeaders, getPaginatedResults } from './paginationHelper';
import { HttpClient } from '@angular/common/http';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/User';
import { PresenceService } from './presence.service';
import { BehaviorSubject, take } from 'rxjs';
import { group } from '@angular/animations';
import { Group } from '../_models/Group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  private hubConnection?: HubConnection;



  constructor(private http: HttpClient, private presenceService: PresenceService) { }


  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .configureLogging("debug")
      .build()

      this.onMessageConnection(otherUsername);
  }


  startHubConnection() {
    if (this.hubConnection)
      this.hubConnection.start().catch(error => console.error(error));
  }

  stopHubConnection() {
    if (this.hubConnection)
      this.hubConnection.stop().catch(error => console.error(error));
  }


  onMessageConnection(otherUsername: string) {
    if (this.hubConnection) {
      this.hubConnection.on("ReceiveMessageThread", messages => {
        this.messageThreadSource.next(messages);
      });

      this.hubConnection.on("UpdatedGroup", (group: Group) => {
        if (group.connections.some(x => x.username === otherUsername)) {
          this.messageThread$.pipe(take(1)).subscribe({
            next: messages => {
              messages.forEach(message => {
                if (!message.dateRead) {
                  message.dateRead = new Date(Date.now())
                }
              })
              this.messageThreadSource.next([...messages])
            }
          })
        }
      })     

      this.hubConnection.on("NewMessage", message => {
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages => {
            this.messageThreadSource.next([...messages, message]);
          }
        })
      });
    }
  }


  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResults<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }


  async sendMessage(username: string, content: string) {
    if (this.hubConnection) {
      const message = {
        recipientUsename: username,
        content
      };
      return this.hubConnection.invoke('SendMessage', {recipientUsername: username, content})
        .catch(error => console.error(error));
    }
  }


  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
