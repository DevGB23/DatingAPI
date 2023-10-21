import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit{
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  messages: Message[] = [];
  images: GalleryItem[] = []; 
  loading = false;

  activeTab: TabDirective | undefined;

  constructor(
    private messageService: MessageService,
    private route: ActivatedRoute){}

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
      console.log("loading messages...");
      
      this.loadMessages();
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

}
