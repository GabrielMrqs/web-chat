class ChatMessage {
    constructor(roomId, messageId, senderId, content) {
        this.roomId = roomId;
        this.messageId = messageId;
        this.senderId = senderId;
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