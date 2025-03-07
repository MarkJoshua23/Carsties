"use server";
import { auth } from "@/auth";
import { fetchWrapper } from "@/lib/fetchWrapper";
//put use server to specify its a server functions
import { Auction, PagedResult } from "@/types";
import { revalidatePath } from "next/cache";
import { FieldValues } from "react-hook-form";

//this will also cache
//it will promise of return of pagedresult with items including a type of auction
export async function getData(query: string): Promise<PagedResult<Auction>> {
    return await fetchWrapper.get(`search${query}`);
}

export async function updateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 10000) + 1,
    };

    return await fetchWrapper.put(
        "auctions/6a5011a1-fe1f-47df-9a32-b5346b289391",
        data
    );
}
//fieldvaues from the form
export async function createAuction(data: FieldValues) {
    return await fetchWrapper.post("auctions", data);
}

export async function getDetailedViewData(id: string): Promise<Auction> {
    return await fetchWrapper.get(`auctions/${id}`);
}

export async function updateAuction(data: FieldValues, id: string) {
    const res = await fetchWrapper.put(`auctions/${id}`, data);
    revalidatePath(`/auctions${id}`);
    return res;
}

export async function deleteAuction(id: string) {
    return await fetchWrapper.del(`auctions/${id}`);
}
