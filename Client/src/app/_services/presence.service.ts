import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs';
import { Message } from '../_models/message';


@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastrService: ToastrService) { }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .configureLogging("debug")
      .withAutomaticReconnect()
      .build();
      
      this.onUsersConnection(this.hubConnection);
    }
    
    startHubConnection() {
      if (this.hubConnection)
        this.hubConnection.start().catch(error => console.error(error));
    }
    
    onUsersConnection(hubConnection: HubConnection) {
      if (this.hubConnection) {
        
        this.hubConnection.on("UserIsOnline", (username) => {
          this.toastrService.info(username + ' has connected');
        });

        this.hubConnection.on("UserIsOffline", (username) => {
          this.toastrService.warning(username + ' has disconnected');
        });

        this.hubConnection.on("GetOnlineUsers", usernameList => {
          this.onlineUsersSource.next(usernameList);
        })
      }
    }
  
  
    stopHubConnection() {
      if (this.hubConnection)
        this.hubConnection.stop().catch(error => console.error(error));
    }

}
