import { Component, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit{
  messages: Message[] | undefined = [];
  pagination?: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(private messageService: MessageService) { 
    this.messages = [];
  }
  
  ngOnInit(): void {
    this.loadMessages();
  }


  loadMessages() {
    this.loading = true;
    this.messageService.getMessages(this.pageSize, this.pageNumber, this.container).subscribe({
      next: response => {
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false;
      }
    })
  }


  deleteMessage(id: number) {
    // this.confirmService.confirm('Confirm delete message', 'This cannot be undone').subscribe(result => {
    //   if (result) {
    //     this.messageService.deleteMessage(id).subscribe(() => {
    //       this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
    //     })
    //   }
    // })

  }


  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

}
