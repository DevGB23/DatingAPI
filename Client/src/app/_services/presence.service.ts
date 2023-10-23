import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/User';


@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  constructor(private toastrService: ToastrService) { }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .configureLogging("debug")
      .withAutomaticReconnect()
      .build();
      
      this.onConnection();
    }
    
    startHubConnection() {
      if (this.hubConnection) {
      this.hubConnection.start().catch(error => console.error(error));
      }
    }
    
    onConnection() {
      if (this.hubConnection) {
        
        this.hubConnection.on("UserIsOnline", (username) => {
          this.toastrService.info(username + ' has connected');
        });

        this.hubConnection.on("UserIsOffline", (username) => {
          this.toastrService.warning(username + ' has disconnected');
        });
      }

  
  
  }
  
  
  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.error(error));
  }



}
