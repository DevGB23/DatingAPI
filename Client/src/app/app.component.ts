import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/User';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any; 

  constructor(private http: HttpClient, private accountSvc: AccountService) {}
  
  ngOnInit(): void {
    this.getUsers();
    this.setCurrenUser();
  }

  getUsers(){
    this.http.get("https://localhost:7287/api/users").subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log("Request Complete"),
    });
  }


  setCurrenUser(){
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user: User = JSON.parse(userString);
    this.accountSvc.setCurrentUser(user);
  }
}
