import AuthPage from "../pages/AuthPage/AuthPage";
import {BrowserRouter, Navigate, Route, Routes, RedirectFunction } from "react-router-dom";
import GamesPage from "../pages/GamesPage/GamesPage";
import GamePage from "../pages/GamePage/GamePage";
import RatingPage from "../pages/RatingPage/RatingPage";

const CustomRouter = ({isAuthenticated}) => {

    return (
        <BrowserRouter>
            <div className="flex justify-center items-center h-screen bg-gray-100">
                <Routes>
                    {
                        isAuthenticated
                            ? <>
                                <Route path="/" element={<GamesPage />} />
                                <Route path="/game/:id" element={<GamePage />} />
                                <Route path="/rating" element={<RatingPage />} />
                            </>
                            : <>
                                <Route path="/register" element={<AuthPage isRegistration={true} />} />
                                <Route path="/login" element={<AuthPage isRegistration={false} />} />
                            </>
                    }
                </Routes>
            </div>
        </BrowserRouter>
    );
}

export default CustomRouter;