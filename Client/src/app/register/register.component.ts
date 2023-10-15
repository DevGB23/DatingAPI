import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter(); 
  model: any = {};

  constructor(private accountSvc: AccountService, private toastr: ToastrService){}

  ngOnInit(): void {
  }

  register(){
    this.accountSvc.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: error => {
        this.toastr.error(error.error);
        console.error(error);
        
      },
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }



}
