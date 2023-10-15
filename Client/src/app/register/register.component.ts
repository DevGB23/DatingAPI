import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter(); 
  model: any = {};

  constructor(private accountSvc: AccountService){}

  ngOnInit(): void {
  }

  register(){
    this.accountSvc.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: error => console.error(error),
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }



}
