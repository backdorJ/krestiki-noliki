import React, { useState } from "react";
import "./CreateGameModal.css";
import {createGame} from "../../http/gameHttp";
import {useNavigate} from "react-router-dom";

const CreateGameModal = ({ isOpen, onClose, onSubmit }) => {
    const [roomName, setRoomName] = useState("");
    const [maxRating, setMaxRating] = useState("");
    const navigate = useNavigate();

    if (!isOpen) return null;

    const handleSubmit = () => {
        const request = {
            roomName,
            maxRating
        }

        createGame(request).then((response) => {
            if (response.status === 200) {
                navigate("game/" + response.data.id);
            }
        })

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
