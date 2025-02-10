import {$authClient, $client} from "./index";

const baseUrl = "game/"

export const createGame = (body) => {
    return $authClient.post(baseUrl + "create-game", body);
}

export const getGames = (body) => {
    return $authClient.get(baseUrl + "get-games");
}