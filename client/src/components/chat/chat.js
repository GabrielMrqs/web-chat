import React, { useState, useEffect } from 'react';
import signalRService from '../../services/signalRService';
import { HubConnectionState } from '@microsoft/signalr';
import './chat.css';
import chatService from '../../services/chatService';
import { useParams } from 'react-router-dom';

function Chat() {
    const { id: roomId } = useParams();
    const [messages, setMessages] = useState([]);
    const [newMessage, setNewMessage] = useState('');
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        if (!roomId) {
            return;
        }

        const connect = async () => {
            if (!signalRService.connection || signalRService.connection.state === HubConnectionState.Disconnected) {
                const connected = await signalRService.startConnection();
                setIsConnected(connected);
            }
        };

        connect();

        const getHistory = async () => {
            var history = await chatService.getHistory(roomId);
            if (history.data) {
                setMessages(prev => [...prev, ...history.data]);
            }
        }

        getHistory();

        const messageHandler = (message) => {
            setMessages(prev => [...prev, message]);
        };

        signalRService.onReceiveMessage(messageHandler);

        return () => {
            if (signalRService.connection?.state === HubConnectionState.Connected) {
                signalRService.stopConnection();
            }

            if (signalRService.connection) {
                signalRService.connection.off('ReceiveMessage', messageHandler);
            }
        };
    }, [roomId]);

    const handleSendMessage = async (e) => {
        e.preventDefault();
        if (newMessage.trim() && isConnected) {
            const sent = await signalRService.sendMessage(roomId, newMessage);
            if (sent) {
                setNewMessage('');
            }
        }
    };

    const formatTimestamp = (timestamp) => {
        return new Date(timestamp).toLocaleTimeString();
    };

    return (
        <div className="chat-container">
            <div className="chat-header">
                <h2>Web Chat</h2>
                <div className={`connection-status ${isConnected ? 'connected' : 'disconnected'}`}>
                    {isConnected ? 'Connected' : 'Disconnected'}
                </div>
            </div>
            <div className="messages-container">
                {messages.map((message, index) => (
                    <div key={message.messageId} className="message">
                        <div className="message-header">
                            <span className="message-user">User: {message.senderId}...</span>
                            <span className="message-time">{formatTimestamp(message.sentAt)}</span>
                        </div>
                        <div className="message-content">{message.content}</div>
                    </div>
                ))}
            </div>
            <form onSubmit={handleSendMessage} className="message-form">
                <input
                    type="text"
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                    placeholder="Type your message here..."
                    className="message-input"
                />
                <button
                    type="submit"
                    className="send-button"
                    disabled={!isConnected || !newMessage.trim()}
                >
                    Send
                </button>
            </form>
        </div>
    );
}

export default Chat; 