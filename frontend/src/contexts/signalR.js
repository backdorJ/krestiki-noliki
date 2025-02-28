import React, { createContext, useState, useEffect, useContext } from 'react';
import {HttpTransportType, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

const SignalRContext = createContext();

export const useSignalR = () => {
    return useContext(SignalRContext);
};

export const SignalRProvider = ({ children }) => {
    const [connection, setConnection] = useState(null);
    const [connected, setConnected] = useState(false);

    const startConnection = () => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("http://localhost:7000/game-hub", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets,
                accessTokenFactory: () => localStorage.getItem('token') ?? "default"
            })
            .build();

        newConnection.start()
            .then(() => setConnected(true))
            .catch((err) => console.error("Connection failed: ", err));

        setConnection(newConnection);
    }

    return (
        <SignalRContext.Provider value={{ connection, connected, startConnection }}>
            {children}
        </SignalRContext.Provider>
    );
};