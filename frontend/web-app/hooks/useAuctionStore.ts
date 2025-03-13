import { Auction, PagedResult } from "@/types";
import { create } from "zustand";

type State = {
    auctions: Auction[];
    totalCount: number;
    pageCount: number;
};

type Actions = {
    setData: (data: PagedResult<Auction>) => void;
    //update the highest bid displayed
    setCurrentPrice: (auctionId: string, amount: number) => void;
};

const initialState: State = {
    auctions: [],
    pageCount: 0,
    totalCount: 0,
};
//pass the types in the <>
export const useAuctionStore = create<State & Actions>((set) => ({
    ...initialState,
    setData: (data: PagedResult<Auction>) => {
        set(() => ({
            auctions: data.results,
            totalCount: data.totalCount,
            pageCount: data.pageCount,
        }));
    },
    setCurrentPrice: (auctionId: string, amount: number) => {
        //we need state since we want to have the existing values and replace just the price not all values
        set((state) => ({
            //map iterates all auctions
            auctions: state.auctions.map((auction) =>
                auction.id === auctionId
                    ? //spread all the values of each auction and change only currentHighBid
                      { ...auction, currentHighBid: amount }
                    : auction
            ),
        }));
    },
}));
