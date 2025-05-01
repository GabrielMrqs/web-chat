import * as signalR from '@microsoft/signalr';
import ChatMessage from '../models/chatMessage';

class SignalRService {
    constructor() {
        this.connection = null;
        this.hubUrl = 'https://localhost:44352/chatHub';
    }

    async startConnection() {
        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(this.hubUrl, {
                    skipNegotiation: true,
                    transport: signalR.HttpTransportType.WebSockets
                })
                .withAutomaticReconnect()
                .build();

            await this.connection.start();
            console.log('SignalR Connected');
            return true;
        } catch (err) {
            console.error('SignalR Connection Error:', err);
            return false;
        }
    }

    async sendMessage(roomId, content) {
        try {
            if (this.connection?.state === signalR.HubConnectionState.Connected) {
                const chatMessage = new ChatMessage(roomId, null, null, content);
                await this.connection.invoke('SendMessage', chatMessage);
                return true;
            }
            return false;
        } catch (err) {
            console.error('Error sending message:', err);
            return false;
        }
    }

    onReceiveMessage(callback) {
        if (this.connection) {
            this.connection.on('ReceiveMessage', (messageJson) => {
                const message = ChatMessage.fromJson(messageJson);
                callback(message);
            });
        }
    }

    async stopConnection() {
        try {
            if (this.connection) {
                await this.connection.stop();
                console.log('SignalR Disconnected');
            }
        } catch (err) {
            console.error('Error disconnecting:', err);
        }
    }
}

const signalRService = new SignalRService(); 

export default signalRService;