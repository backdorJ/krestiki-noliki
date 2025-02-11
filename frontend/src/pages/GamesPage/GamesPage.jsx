import React, {useEffect, useState} from 'react';
import './GamesPage.css';
import {useSignalR} from "../../contexts/signalR";
import {createGame, getGames, joinGame} from "../../http/gameHttp";
import CreateGameModal from "../../components/CreateGameModal/CreateGameModal";
import {useNavigate} from "react-router-dom";

const GamesPage = () => {
    const [games, setGames] = useState([]);
    const { connection, connected, startConnection } = useSignalR()
    const [isModalOpen, setIsModalOpen] = useState();
    const navigate = useNavigate();

    useEffect(() => {
        if (!connected) {
            startConnection();
        }
    }, []);

    useEffect(() => {
        if (connection)
        {
            connection.on("GetCreatedGameNotify", response => {
                if (response.arguments)
                    console.log(response);
                    setGames(prev => [...prev, response]);
            });
        }
    }, [connection]);

    useEffect(() => {
        console.log(localStorage)
        getGames()
            .then(response => {
                if (response.status === 200) {
                    setGames(response.data.items);
                }
            })
    }, []);

    const startGame = () => {
        setIsModalOpen(true)
    };

    const join = (gameId) => {
        navigate(`/game/${gameId}`);
    };

    return (
        <div className="game-list-container">
            <div className="game-list-header">
                <h1>Games Lobby</h1>
                <div style={{ display: "flex", flexDirection: "column" }}>
                    <button style={{marginBottom: "10px"}} className="create-game-button" onClick={startGame}>Create Game</button>
                    <button className="create-game-button" onClick={() => navigate("/rating")}>Rating</button>
                </div>
            </div>
            <div className="game-list">
                {games.map((game) => (
                    <div key={game.id} className="game-card">
                        <div className="game-card-content">
                            <h3>{game.createUsername}</h3>
                            <p>Status: {game.status}</p>
                        </div>
                        <button className="join-game-button" onClick={() => join(game.gameId)}>
                            Join Game
                        </button>
                    </div>
                ))}
            </div>
            <CreateGameModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </div>
    );
};

export default GamesPage;