"use server";
//put use server to specify its a server functions
import { Auction, PagedResult } from "@/types";

//this will also cache
//it will promise of return of pagedresult with items including a type of auction
export async function getData(query: string): Promise<PagedResult<Auction>> {
    const res = await fetch(`http://localhost:6001/search${query}`);
    if (!res.ok) throw new Error("Failed to fetch data");
    //get the respose from res
    return res.json();
}

export async function updateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 10000) + 1,
    };

    //gateway server
    const res = await fetch("http://localhost:6001/auctions", {
        method: "PUT",
        headers: {},
        body: JSON.stringify(data),
    });

    if (!res.ok) return { status: res.status, message: res.statusText };

    return res.json();
}
