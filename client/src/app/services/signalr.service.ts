import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Message } from '../models/message.interface';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection;
  private messageSubject = new Subject<Message>();
  public message$ = this.messageSubject.asObservable();

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:44352/chatHub')
      .withAutomaticReconnect()
      .build();

    this.startConnection();
    this.registerOnMessageReceived();
  }

  private startConnection() {
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  private registerOnMessageReceived() {
    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      this.messageSubject.next(message);
    });
  }

  public sendMessage(message: Message) {
    this.hubConnection.invoke('SendMessage', message)
      .catch(err => console.error(err));
  }
} 