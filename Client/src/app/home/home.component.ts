import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  baseUrl = environment.apiUrl;
  registerMode = false;

  users: any;

  constructor(private http: HttpClient){}

  ngOnInit(): void {
  }

  registerToggle(){
    this.registerMode = !this.registerMode
  }

  getUsers(){

    this.http.get(this.baseUrl + 'users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log("Request Complete"),
    });
  }

  cancelRegisterMode(event: boolean){
    this.registerMode = event;
  }

}
