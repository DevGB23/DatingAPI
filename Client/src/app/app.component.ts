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

  constructor(private accountSvc: AccountService) {}
  
  ngOnInit(): void {
    this.setCurrenUser();
  }

  setCurrenUser(){
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user: User = JSON.parse(userString);
    this.accountSvc.setCurrentUser(user);
  }
}
