import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/User';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
  model: any = {};
  
  constructor(public accountService: AccountService){}


  ngOnInit(): void {
  }

  getCurrentUser() {
    this.accountService.currentUser$.subscribe({
      error: error => console.error(),
    })
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response)
      },
      error: error => console.error(error)      
    });
  }

  logout(){
    this.accountService.logout();
  }


}
