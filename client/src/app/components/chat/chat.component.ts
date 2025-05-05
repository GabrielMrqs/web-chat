import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SignalRService } from '../../services/signalr.service';
import { Message } from '../../models/message.interface';
import { MessageService } from '../../services/message.service';
import { ActivatedRoute } from '@angular/router';
import { distinctUntilChanged, filter, map, merge, Observable, scan, shareReplay, startWith, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {
  message: string = '';
  roomId: string = '';
  messages$!: Observable<Message[]>;

  constructor(
    private signalRService: SignalRService,
    private messageService: MessageService,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.messages$ = this.getMessages();
  }

  private getMessages(): Observable<Message[]> {
    return this.route.paramMap.pipe(
      map(pm => pm.get('roomId')!),
      distinctUntilChanged(),
      tap(id => this.roomId = id),
      switchMap(roomId =>
        this.messageService.getMessages(roomId).pipe(
          switchMap(initialMsgs =>
            this.signalRService.message$.pipe(
              filter(m => m.roomId === roomId),
              startWith(...initialMsgs)
            )
          ),
          scan((all, one) => [...all, one], [] as Message[])
        )
      )
    );
  }

  sendMessage() {
    if (this.message.trim()) {
      const newMessage: Message = {
        roomId: this.roomId,
        content: this.message
      };
      this.signalRService.sendMessage(newMessage);
      this.message = '';
    }
  }
} 