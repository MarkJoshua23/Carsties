import { auth } from "@/auth";
import { error } from "console";
import { headers } from "next/headers";

const baseUrl = "http://localhost:6001/";

async function get(url: string) {
    const requestOptions = {
        method: "GET",
        headers: await getHeaders(),
    };

    const response = await fetch(baseUrl + url, requestOptions);
    return handleResponse(response);
}
async function post(url: string, body: {}) {
    const requestOptions = {
        method: "POST",
        headers: await getHeaders(),
        body: JSON.stringify(body),
    };

    const response = await fetch(baseUrl + url, requestOptions);

    return handleResponse(response);
}
async function put(url: string, body: {}) {
    const requestOptions = {
        method: "PUT",
        headers: await getHeaders(),
        body: JSON.stringify(body),
    };

    const response = await fetch(baseUrl + url, requestOptions);

    return handleResponse(response);
}
async function del(url: string) {
    const requestOptions = {
        method: "DELETE",
        headers: await getHeaders(),
    };
    const fullUrl = baseUrl + url;
    console.log("DELETE request to:", fullUrl);
    const response = await fetch(baseUrl + url, requestOptions);

    return handleResponse(response);
}

async function handleResponse(response: Response) {
    const text = await response.text();
    //parse text if its not null or empty string
    const data = text && JSON.parse(text);
    console.log(response);
    if (response.ok) {
        // console.log(response);
        return data || response.statusText;
    } else {
        const error = {
            status: response.status,
            message: response.statusText,
        };
        //return error as an object
        return { error };
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
    return headers;
}

export const fetchWrapper = {
    get,
    post,
    put,
    del,
};
