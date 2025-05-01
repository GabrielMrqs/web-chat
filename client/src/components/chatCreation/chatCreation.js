import React, { useState, useEffect } from "react";
import chatService from "../../services/chatService";
import { useNavigate } from "react-router-dom";
import "./chatCreation.css";

function ChatCreation() {
    let navigate = useNavigate();
    const [roomName, setRoomName] = useState("");
    const [rooms, setRooms] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    const fetchRooms = async () => {
        setIsLoading(true);
        try {
            const res = await chatService.getRooms();
            setRooms(res.data);
        } catch (err) {
            console.error("Error when searching for rooms:", err);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchRooms();
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!roomName.trim()) {
            return;
        }

        try {
            await chatService.createRoom(roomName);
            setRoomName("");
            fetchRooms();
        } catch (err) {
            console.error("Err", err);
        }
    };

    return (
        <div className="chat-creation-container">
            <h2>Create a new chat</h2>
            <form onSubmit={handleSubmit} className="creation-form">
                <div className="form-group">
                    <label htmlFor="roomName">Name:</label>
                    <input
                        id="roomName"
                        type="text"
                        value={roomName}
                        onChange={e => setRoomName(e.target.value)}
                        placeholder="Enter room name"
                    />
                    <button type="submit">Create Room</button>
                </div>
            </form>

            <h2>Available rooms</h2>
            {isLoading ? (
                <p className="loading-message">Loading rooms...</p>
            ) : (
                <table className="rooms-table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Created At</th>
                            <th> </th>
                        </tr>
                    </thead>
                    <tbody>
                        {rooms.map(r => (
                            <tr key={r.roomId}>
                                <td>{r.name}</td>
                                <td>{new Date(r.createdAt).toLocaleString()}</td>
                                <td>
                                    <button onClick={() => navigate(`${r.roomId}`)}>Join</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default ChatCreation;