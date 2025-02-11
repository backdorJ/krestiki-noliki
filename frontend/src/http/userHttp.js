import {$authClient, $client} from "./index";

const baseUrl = "user/"

export const getRating = () => {
    return $authClient.get(baseUrl + "get-rate-users");
}