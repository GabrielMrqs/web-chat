class ChatRoom {
    constructor(roomId, name, createdAt) {
        this.roomId = roomId;
        this.name = name;
        this.createdAt = new Date(createdAt);
    }

    static fromJson(json) {
        return new ChatRoom(
            json.roomId,
            json.name,
            new Date(json.createdAt)
        )
    }
}

export default ChatRoom; 