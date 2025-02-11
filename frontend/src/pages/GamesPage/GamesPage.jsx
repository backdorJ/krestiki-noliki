import React, {useEffect, useState} from 'react';
import './GamesPage.css';
import {useSignalR} from "../../contexts/signalR";
import {createGame, getGames, getGame} from "../../http/gameHttp";
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
                if (response)
                    setGames(prev => [...prev, response].sort((a, b) => {
                    // Сначала сортируем по статусу (например, "active" выше "inactive")
                    if (a.status !== b.status) {
                        return a.status > b.status ? -1 : 1; // Изменить порядок при необходимости
                    }
                    // Затем сортируем по дате (предполагается, что date - строка ISO)
                    return new Date(b.date) - new Date(a.date);
                }));
            });
        }
    }, [connection]);

    useEffect(() => {
        getGames()
            .then(response => {
                if (response.status === 200) {
                    setGames(response.data.items.sort((a, b) => {
                        // Сначала сортируем по статусу (например, "active" выше "inactive")
                        if (a.status !== b.status) {
                            return a.status > b.status ? -1 : 1; // Изменить порядок при необходимости
                        }
                        // Затем сортируем по дате (предполагается, что date - строка ISO)
                        return new Date(b.date) - new Date(a.date);
                    }));
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
                            {
                                (new Date(game.createdAt)).toISOString().split("T")
                                    .map(x => <p>{x.replace("Z", "")}</p>)
                            }
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