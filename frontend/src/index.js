import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import {SignalRProvider} from "./contexts/signalR";
import {AuthProvider} from "./contexts/AuthContext";

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
        <AuthProvider>
            <SignalRProvider>
                <App/>
            </SignalRProvider>
        </AuthProvider>
);
