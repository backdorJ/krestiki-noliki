import React, { useState } from "react";
import "./CreateGameModal.css";
import {createGame} from "../../http/gameHttp";

const CreateGameModal = ({ isOpen, onClose, onSubmit }) => {
    const [roomName, setRoomName] = useState("");
    const [maxRating, setMaxRating] = useState("");

    if (!isOpen) return null;

    const handleSubmit = () => {
        const request = {
            roomName,
            maxRating
        }

        createGame(request)

        onClose();
    };

    return (
        <div className="modal-overlay">
            <div className="modal">
                <h2>Create Game</h2>
                <form onSubmit={handleSubmit}>
                    <label>
                        Room Name:
                        <input
                            type="text"
                            value={roomName}
                            onChange={(e) => setRoomName(e.target.value)}
                            required
                        />
                    </label>
                    <label>
                        Max Rating:
                        <input
                            type="number"
                            value={maxRating}
                            onChange={(e) => setMaxRating(e.target.value)}
                            required
                        />
                    </label>
                    <div className="modal-buttons">
                        <button onClick={handleSubmit} className="btn-create">Create</button>
                        <button type="button" className="btn-cancel" onClick={onClose}>Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default CreateGameModal;
