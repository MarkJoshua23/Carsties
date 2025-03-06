import { auth } from "@/auth";
import { error } from "console";
import { headers } from "next/headers";

const baseUrl = "http://localhost:6001/";

async function get(url: string) {
    const requestOptions = {
        method: "GET",
        headers: {},
    };

    const response = await fetch(baseUrl + url, requestOptions);

    return handleResponse(response);
}
async function post(url: string, body: {}) {
    const requestOptions = {
        method: "POST",
        headers: {},
        body: JSON.stringify(body),
    };

    const response = await fetch(baseUrl + url, requestOptions);

    return handleResponse(response);
}

async function handleResponse(response: Response) {
    const text = await response.text();
    const data = text && JSON.parse(text);

    if (response.ok) {
        return data || response.statusText;
    } else {
        const error = {
            status: response.status,
            message: response.statusText,
        };
        return error;
    }
}

async function getHeaders() {
    const session = await auth();
    const headers = {
        "Content-type": "application/json",
    } as any;
    //add token to headers if theres a token
    if (session?.accessToken) {
        headers.Authorization = "Bearer " + session.accessToken;
    }
}

export const fetchWrapper = {
    get,
};
