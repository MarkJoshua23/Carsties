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
