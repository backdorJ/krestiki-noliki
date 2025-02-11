import React, { useState, useEffect } from "react";
import "./CreateGameModal.css";
import {useSignalR} from "../../contexts/signalR";
import {createGame, getGame} from "../../http/gameHttp";
import {useNavigate} from "react-router-dom";

const CreateGameModal = ({ isOpen, onClose, onSubmit }) => {
    const { connection, connected, startConnection } = useSignalR()
    const [maxRating, setMaxRating] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        if (!connected) {
            startConnection();
        }
    }, []);

    if (!isOpen) return null;

    const handleSubmit = () => {
        const request = {
            roomName: "",
            maxRating
        }

        createGame(request).then((response) => {
            if (response.status === 200) {
                connection.invoke("JoinRoom", response.data.id)
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
