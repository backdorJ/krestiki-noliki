import {useEffect, useState} from "react";
import './GamePage.css'
import {useSignalR} from "../../contexts/signalR";
import {useParams} from "react-router-dom";


const GamePage = () => {
    // Хранение выбора игрока
    const { id } = useParams()
    const [playerChoice, setPlayerChoice] = useState(null);
    const [opponentChoice, setOpponentChoice] = useState(null);
    const [gameOver, setGameOver] = useState(false);
    const { connection, connected, startConnection } = useSignalR()
    const [isGameStarted, setIsGameStarted] = useState(false);
    const [isPlayer, setIsPlayer] = useState();
    const [logs, setLogs] = useState([]);

    const choices = ['rock', 'paper', 'scissors'];

    // Логика для обработки выбора
    const handlePlayerChoice = (choice) => {
        connection.invoke("MakeMove", id, choice)
    };

    // Сброс игры
    const handleResetGame = () => {
        setPlayerChoice(null);
        setOpponentChoice(null);
        setGameOver(false);
    };

    useEffect(() => {
        if (connection) {
            connection.on("JoinedGameInfo", response => {
                setIsGameStarted(response.status === 2)
                setIsPlayer(response.isPlayer)
            })

            connection.on("MoveMade", response => {
                setLogs(prev => [...prev, response.message]);
            })

            connection.on("GameResult", response => {
                setLogs(prev => [...prev, response.message]);
            })
        }
    }, [connection]);



    return (
        <div className="game-container">
            <h1>Rock, Paper, Scissors</h1>

            <div className="choices">
                {!gameOver && isPlayer && isGameStarted && (
                    choices.map((choice) => (
                        <button
                            key={choice}
                            onClick={() => handlePlayerChoice(choice)}
                        >
                            {choice}
                        </button>
                    ))
                )}

                {gameOver && (
                    <div className="result">
                        <p>You chose: {playerChoice}</p>
                        <p>Opponent chose: {opponentChoice}</p>
                    </div>
                )}
            </div>
            <div>
                {
                    logs.map(log => <p>{log}</p>)
                }
            </div>
        </div>
    );
};

export default GamePage