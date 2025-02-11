import {useEffect, useState} from "react";
import './GamePage.css'
import {useSignalR} from "../../contexts/signalR";
import {useNavigate, useParams} from "react-router-dom";
import {getGame} from "../../http/gameHttp";


const GamePage = () => {
    // Хранение выбора игрока
    const { id } = useParams()
    const { connection, connected, startConnection } = useSignalR()
    const [isGameStarted, setIsGameStarted] = useState(false);
    const [isPlayer, setIsPlayer] = useState(false);
    const [logs, setLogs] = useState([]);
    const [madeMove, setMadeMove] = useState(false)
    const navigate = useNavigate();
    const [isFinished, setIsFinished] = useState(false);

    const choices = ['rock', 'paper', 'scissors'];

    // Логика для обработки выбора
    const handlePlayerChoice = (choice) => {
        connection.invoke("MakeMove", id, choice)
        setMadeMove(true)
    };

    useEffect(() => {
        getGame(id)
            .then(response => {
                if (response.status === 200) {
                    if (response.data.moves) {
                        const newLogs = [...response.data.moves]
                        if (response.data.winnerName) {
                            newLogs.push(`Победа: ${response.data.winnerName}`)
                        }
                        else if (response.data.status === "Finished") {
                            newLogs.push('Ничья')
                        }
                        setLogs(newLogs)
                    }

                    const isFinished = response.data.status === "Finished"
                    if (!isFinished) {
                        if (!isPlayer)
                            (new Promise(r => setTimeout(r, 1000)))
                                .then(() => connection.invoke("JoinRoom", id));
                        else
                            connection.invoke("JoinRoom", id)
                    }

                    setIsFinished(isFinished);
                }
            })
    }, [id]);

    useEffect(() => {
        if (connection && !isFinished) {
            connection.on("JoinedGameInfo", response => {
                setIsGameStarted(response.status === 2)
                setIsPlayer(response.isPlayer)
            })

            connection.on("MoveMade", response => {
                setLogs(prev => [response.message]);
            })

            connection.on("GameResult", response => {
                getGame(id)
                    .then(response => {
                        if (response.status === 200) {
                            if (response.data.moves) {
                                const newLogs = [...response.data.moves]
                                if (response.data.winnerName) {
                                    newLogs.push(`Победа: ${response.data.winnerName}`)
                                }
                                else if (response.data.status === "Finished") {
                                    newLogs.push('Ничья')
                                }
                                setLogs(newLogs)
                            }

                            setIsFinished(true);
                        }
                    })
            })

            connection.on("NewRoundStarted", response => {
                setLogs([])
                setMadeMove(false)
                (new Promise(r => setTimeout(r, 3000)))
                    .then(() => navigate(`/game/${response}`));
            })
        }
    }, [connection, isFinished]);

    return (
        <div className="game-container">
            <button className="button leave-button" onClick={() => navigate("/")}>Back to Game List</button>
            <h1>Rock, Paper, Scissors</h1>

            <div className="choices">
                {isPlayer && isGameStarted && (
                    choices.map((choice) => (
                        <button
                            disabled={madeMove}
                            key={choice}
                            onClick={() => handlePlayerChoice(choice)}
                        >
                            {choice}
                        </button>
                    ))
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