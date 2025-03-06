"use server";
import { auth } from "@/auth";
import { fetchWrapper } from "@/lib/fetchWrapper";
//put use server to specify its a server functions
import { Auction, PagedResult } from "@/types";

//this will also cache
//it will promise of return of pagedresult with items including a type of auction
export async function getData(query: string): Promise<PagedResult<Auction>> {
    return await fetchWrapper.get(`search${query}`);
}

export async function updateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 10000) + 1,
    };

    const session = await auth();
    //gateway server
    const res = await fetch(
        "http://localhost:6001/auctions/6a5011a1-fe1f-47df-9a32-b5346b289391",
        {
            method: "PUT",
            headers: {
                "Content-type": "application/json",
                //dont forget the space after bearer
                Authorization: "Bearer " + session?.accessToken,
            },
            body: JSON.stringify(data),
        }
    );

    if (!res.ok) return { status: res.status, message: res.statusText };

    return res.statusText;
}
