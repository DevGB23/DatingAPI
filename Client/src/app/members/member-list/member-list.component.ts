import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/_models/Pagination';
import { UserParams } from 'src/app/_models/UserParams';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit{
  members: Member[] | undefined;
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{value: 'male', display: 'Male'}, {value: 'female', display: 'Female'}]

  constructor(private memberService: MembersService){
    this.userParams = memberService.getUserParams();    
  }
  
  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    if (this.userParams) {
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    };
  }

  resetFilters() {  
      this.userParams = this.memberService.resetUserParams();
      this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
  
}
