import React, {useEffect, useState} from 'react';
import './GamesPage.css';
import {useSignalR} from "../../contexts/signalR";
import {createGame, getGames} from "../../http/gameHttp";
import JoinGameModal from "../../components/JoinGameModal/JoinGameModal";
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
            connection.on("GetCreatedGameNotify", response => {
                if (response.arguments)
                    console.log(response);
                    setGames(prev => [...prev, response]);

                connection.on("GameStarted", response => {
                    if (response) {
                        navigate("/GamesPage");
                    }
                })
            });
    }, [connection]);

    useEffect(() => {
        getGames()
            .then(response => {
                if (response.status === 200) {
                    setGames(response.data.items.slice(0, 3));
                }
            })
    }, []);

    const startGame = () => {
        setIsModalOpen(true)
    };

    const joinGame = (gameId) => {
        connection.invoke("JoinRoom", gameId);
    };

    return (
        <div className="game-list-container">
            <div className="game-list-header">
                <h1>Games Lobby</h1>
                <button className="create-game-button" onClick={startGame}>Create Game</button>
            </div>
            <div className="game-list">
                {games.map((game) => (
                    <div key={game.id} className="game-card">
                        <div className="game-card-content">
                            <h3>{game.gameId.slice(0, 10)}</h3>
                            <p>Status: {game.status}</p>
                        </div>
                        <button className="join-game-button" onClick={() => joinGame(game.gameId)}>
                            Join Game
                        </button>
                    </div>
                ))}
            </div>
            <JoinGameModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </div>
    );
};

export default GamesPage;