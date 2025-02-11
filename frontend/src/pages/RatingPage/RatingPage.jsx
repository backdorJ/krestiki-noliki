import React, { useState, useEffect } from "react";
import "./RatingPage.css";
import {getRating} from "../../http/userHttp";

const UserRankingPage = () => {
    const [users, setUsers] = useState([]);

    useEffect(() => {
        getRating()
            .then(response => {
                if (response.status === 200) {
                    setUsers(response.data.users);
                }
            })
    }, []);

    return (
        <div className="ranking-container">
            <h2>User Rankings</h2>
            <table className="ranking-table">
                <thead>
                <tr>
                    <th>#</th>
                    <th>Name</th>
                    <th>Rating</th>
                </tr>
                </thead>
                <tbody>
                {users.map((user, index) => (
                    <tr key={user.userId}>
                        <td>{index + 1}</td>
                        <td>{user.username}</td>
                        <td>{user.rating}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default UserRankingPage;
