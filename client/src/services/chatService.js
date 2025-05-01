import axios from 'axios';

class ChatService {
    constructor() {
        this.apiUrl = 'https://localhost:44352/api/';
    }

    async getHistory(roomId) {
        return await axios.get(`${this.apiUrl}Message/${roomId}`,)
    }

    async getRooms() {
        return await axios.get(`${this.apiUrl}Room`)
    }

    async createRoom(name) {
        const params = {
            name: name
        }
        return await axios.post(`${this.apiUrl}Room`, params);
    }
}

const chatService = new ChatService();

export default chatService;