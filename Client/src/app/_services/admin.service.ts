import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/User';
import { AccountService } from './account.service';
import { map } from 'rxjs';
import { UserWithRoles } from '../_models/UserWithRoles';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private acc: AccountService) { }


  getUsersWithRoles() {
    return this.http.get<UserWithRoles[]>(this.baseUrl + 'admin/users-with-roles').pipe(
      map(x => x)
    );
  }

  
  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {});
  }
}
