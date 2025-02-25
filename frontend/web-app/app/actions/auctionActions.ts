"use server";
//put use server to specify its a server functions
import { Auction, PagedResult } from "@/types";

//this will also cache
//it will promise of return of pagedresult with items including a type of auction
export async function getData(
  pageNumber: number = 1
): Promise<PagedResult<Auction>> {
  const res = await fetch(
    `http://localhost:6001/search?pageSize=4&pageNumber=${pageNumber}`,
    {
      next: { revalidate: 60 }, // Cache for 60 seconds, then refresh
    }
  );
  if (!res.ok) throw new Error("Failed to fetch data");
  //get the respose from res
  return res.json();
}
