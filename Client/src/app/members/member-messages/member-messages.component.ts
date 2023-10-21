import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { GalleryModule } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit{
  @ViewChild('messageForm') messageForm: NgForm | undefined;
  @Input() messages: Message[] = [];
  @Input() username: string | undefined;
  messageContent: string = "";
  // loading = false;

  constructor(private messageService: MessageService) { }
 
  ngOnInit(): void {
    
  }


  loadMessages() {
    // // this.loading = true;
    // if (this.username) {
    //   this.messageService.getMessageThread(this.username).subscribe({
    //     next: response => {
    //       this.messages = response;
    //       // this.loading = false;
    //     }
    //   })

    // }
  }

  sendMessage() {
    // this.loading = true;
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        // this.loading = false
        this.messages.push(message)
        this.messageForm?.reset()
      }
    })
  }
}
