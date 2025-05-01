import { v4 as uuidv4 } from 'uuid';

class ChatMessage {
    constructor(roomId, messageId, senderId, content) {
        this.roomId = roomId || uuidv4();
        this.messageId = messageId || uuidv4();
        this.senderId = senderId || uuidv4();
        this.sentAt = new Date();
        this.content = content;
    }

    static fromJson(json) {
        const message = new ChatMessage();
        message.roomId = json.roomId;
        message.messageId = json.messageId;
        message.senderId = json.senderId;
        message.content = json.content;
        message.sentAt = new Date(json.sentAt);
        return message;
    }
}

export default ChatMessage; 