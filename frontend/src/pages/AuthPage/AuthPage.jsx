import { useState } from "react";
import './AuthPage.css'
import {login, register} from "../../http/authHttp";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../../contexts/AuthContext";

const AuthPage = (props) => {
    const { isRegistration } = props
    const [name, setName] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();
    const { login: authenticate } = useAuth();

    const handleSubmit = (e) => {
        e.preventDefault();

        if (name.trim() === "" || name.trim() === "")
            return

        const body = {
            name,
            password: password
        }

        if (isRegistration) {
            body.passwordConfirm = password;
            register(body).then(response => {
                if (response.status === 200) {
                    navigate("/login");
                }
            })
        }
        else
            login(body)
                .then(response => {
                    if (response.status === 200) {
                        authenticate(response.data.jwtToken)
                        navigate("/");
                    }
                });
    };

    return (
        <div className="body-container">
            <div className="registration-container">
                <h2 className="registration-title">{isRegistration ? "Register" : "Login"}</h2>
                <form onSubmit={handleSubmit} className="registration-form">
                    <input
                        type="text"
                        name="name"
                        placeholder="Username"
                        value={name}
                        onChange={e => setName(e.target.value)}
                        className="input-field"
                    />
                    <input
                        type="password"
                        name="password"
                        placeholder="Password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        className="input-field"
                    />
                    <button onClick={handleSubmit} className="submit-button">
                        {isRegistration ? "Register" : "Login"}
                    </button>
                </form>
            </div>
        </div>
    );
}

export default AuthPage;