import { Bid } from "@/types";
import { create } from "zustand";

type State = {
    bids: Bid[];
};

type Actions = {
    setBids: (bids: Bid[]) => void;
    addBid: (bid: Bid) => void;
};

export const useBidStore = create<State & Actions>((set) => ({
    bids: [],
    //this is the initial fetch of bids
    setBids: (bids: Bid[]) => {
        set(() => ({
            bids,
        }));
    },
    //only add the new bids and not refetch all bids again
    addBid: (bid: Bid) => {
        set((state) => ({
            //if a bid is still not in the store then add it
            bids: !state.bids.find((x) => x.id === bid.id)
                ? [bid, ...state.bids]
                : [...state.bids],
        }));
    },
}));
