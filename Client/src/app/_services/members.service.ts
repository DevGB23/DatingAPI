import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResults } from '../_models/Pagination';
import { UserParams } from '../_models/UserParams';
import { AccountService } from './account.service';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;

  constructor(private http: HttpClient, private accountService: AccountService) {
    accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    })
  }


  getUserParams () {
    return this.userParams;
  }


  setUserParams (params: UserParams) {
    this.userParams = params;
  }

  resetUserParams () {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }


  getMembers(userParams: UserParams){
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    let params = this.getPaginationHeaders(userParams.pageSize, userParams.pageNumber);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedresults<Member[]>(this.baseUrl + 'users', params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    ); 
  }

  
  getMember(username: string){
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username == username)

    if (member) return of(member);
    
    console.log(member);
    
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }


  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users', member)
    .pipe(
      map(() => {
        const index = this.members.indexOf(member);
        console.log(index);
        
        this.members[index] = {...this.members[index], ...member};
      })
    );
  }


  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }


  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }


  private getPaginatedresults<T> (url: string, params: HttpParams) {
    const paginatedResults: PaginatedResults<T> = new PaginatedResults<T>();
    
    return this.http.get<T>(url, { observe: 'response', params })
      .pipe(
        map(response => {
          if (response.body) {
            paginatedResults.result = response.body;
          }
          const pagination = response.headers.get('Pagination');

          if (pagination) {
            paginatedResults.pagination = JSON.parse(pagination);
          }

          return paginatedResults;
        })
      );
  }


  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageSize);
    params = params.append('pageSize', pageNumber);

    return params;
    
  }
  
}
