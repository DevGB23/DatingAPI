import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/User';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';


@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastrService: ToastrService, private router: Router) { }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .configureLogging("debug")
      .withAutomaticReconnect()
      .build();
      
      this.onUsersConnection();
    }
    
    startHubConnection() {
      if (this.hubConnection)
        this.hubConnection.start().catch(error => console.error(error));
    }
    
    
    onUsersConnection() {
      if (this.hubConnection) {
        
        this.hubConnection.on("UserIsOnline", (username) => {
          this.onlineUsers$.pipe(take(1)).subscribe({
            next: usernames => this.onlineUsersSource.next([...usernames, username]) 
          });
        });

        this.hubConnection.on("UserIsOffline", (username) => {
          this.onlineUsers$.pipe(take(1)).subscribe({
            next: usernames => this.onlineUsersSource.next(usernames.filter(x => x !== username))
          })
        });

        this.hubConnection.on("GetOnlineUsers", usernameList => {
          this.onlineUsersSource.next(usernameList);
        })

        this.hubConnection.on("NewMessageReceived", ({username, knownAs}) => {
          this.toastrService.info(username + ' has sent you a new message')
            .onTap
            .pipe(take(1))
            .subscribe({
              next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
            });
        })
      }
    }
  
  
    stopHubConnection() {
      if (this.hubConnection)
        this.hubConnection.stop().catch(error => console.error(error));
    }

}
