import {useEffect, useState} from "react";
import './GamePage.css'


const GamePage = () => {
    // Хранение выбора игрока
    const [playerChoice, setPlayerChoice] = useState(null);
    const [opponentChoice, setOpponentChoice] = useState(null);
    const [gameOver, setGameOver] = useState(false);

    const choices = ['rock', 'paper', 'scissors'];

    // Логика для обработки выбора
    const handlePlayerChoice = (choice) => {
        setPlayerChoice(choice);
        setGameOver(true); // После выбора игрока, заканчиваем игру
        setOpponentChoice(choices[Math.floor(Math.random() * choices.length)]); // Оппонент делает случайный выбор
    };

    // Сброс игры
    const handleResetGame = () => {
        setPlayerChoice(null);
        setOpponentChoice(null);
        setGameOver(false);
    };

    return (
        <div className="game-container">
            <h1>Rock, Paper, Scissors</h1>

            <div className="choices">
                {!gameOver && (
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

            {gameOver && (
                <button onClick={handleResetGame}>Play Again</button>
            )}
        </div>
    );
};

export default GamePage