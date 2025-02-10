import React, { createContext, useState, useContext, useEffect } from 'react';

// Контекст для авторизации
const AuthContext = createContext();

export const useAuth = () => {
    return useContext(AuthContext); // Хук для получения данных из контекста
};

// Провайдер для контекста авторизации
export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false); // Состояние авторизации
    // const [user, setUser] = useState(null); // Данные пользователя

    // Проверяем токен в localStorage при монтировании компонента
    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            setIsAuthenticated(true);
            // setUser({ name: 'User', token }); // Здесь можно подставить логику для получения данных пользователя из токена
        }
    }, []);

    // Функция для авторизации
    const login = (token) => {
        localStorage.setItem('token', token); // Сохраняем токен в localStorage
        setIsAuthenticated(true);
        // setUser(userData); // Устанавливаем данные пользователя
    };

    // Функция для выхода
    const logout = () => {
        localStorage.removeItem('token'); // Удаляем токен
        setIsAuthenticated(false);
        // setUser(null); // Очищаем данные пользователя
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};