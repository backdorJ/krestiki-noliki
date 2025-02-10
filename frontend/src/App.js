import CustomRouter from "./components/CustomRouter";
import {useEffect, useState} from "react";
import {SignalRProvider} from "./contexts/signalR";
import {AuthProvider, useAuth} from "./contexts/AuthContext";

function App() {
    const { isAuthenticated, login, logout } = useAuth();
    const [isAuth, setIsAuth] = useState(false)


    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            login(token)
        } else {
            setIsAuth(false);
        }
    }, []);

  return (
    <>
        <CustomRouter isAuthenticated={isAuthenticated} />
    </>
  );
}

export default App;
