import axios from "axios";

const $client = axios.create({
    baseURL: process.env.REACT_APP_API_URL,
    headers: {
        "Content-Type": "application/json",
    },
})

const $authClient = axios.create({
    baseURL: process.env.REACT_APP_API_URL,
})

function authInterceptor(config) {
    let token = localStorage.getItem('token')
    config.headers.Authorization = `Bearer ${token}`
    return config;
}

$authClient.interceptors.request.use(authInterceptor)

export {
    $client,
    $authClient
}
