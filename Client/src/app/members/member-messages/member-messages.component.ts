import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { GalleryModule } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit{
  @ViewChild('messageForm') messageForm: NgForm | undefined;
  @Input() username: string | undefined;
  messageContent: string = "";
  loading = false;

  constructor(public messageService: MessageService) { }
 
  ngOnInit(): void {
    
  }

  sendMessage() {
    this.loading = true;
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent)
      .then(() => {
        this.messageForm?.reset();
      }).catch(error =>console.error(error)
      )
  }
}
