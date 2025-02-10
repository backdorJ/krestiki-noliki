import {$client} from "./index";

const baseUrl = "account/"

export const register = (body) => {
    return $client.post(baseUrl + "register", body);
}

export const login = (body) => {
    return $client.post(baseUrl + "login", body);
}