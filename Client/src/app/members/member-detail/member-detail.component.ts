import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { AccountService } from 'src/app/_services/account.service';
import { PresenceService } from 'src/app/_services/presence.service';
import { User } from 'src/app/_models/User';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit, OnDestroy{
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  messages: Message[] = [];
  images: GalleryItem[] = []; 
  user: User = {} as User;
  loading = false;

  activeTab: TabDirective | undefined;

  constructor(
    private messageService: MessageService,
    private route: ActivatedRoute,
    private accountService: AccountService,
    public presenceService: PresenceService){
      this.accountService.currentUser$.subscribe({
        next: user => {
          if (user)
            this.user = user
        }
      })
    }
  
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => {
        this.member = data['member']
      }
    })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages();
  }


  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }


  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    
    if (this.activeTab.heading === 'Messages' && this.messages!!.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.username);
      this.messageService.startHubConnection();
    }else {
      this.messageService.stopHubConnection();
    } 
  }


  loadMessages() {
    this.loading = true;
    if (this.member) {
      this.messageService.getMessageThread(this.member.username).subscribe({
        next: response => {
          this.messages = response;
          this.loading = false;
        }
      })
    }
  }

  getImages(){
    if (!this.member) return;
    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({
        src: photo.imageUrl,
        thumb: photo.imageUrl
      }))
    }
  }


  ngOnDestroy(): void {
    this.messageService.stopHubConnection(); 
  }

}
