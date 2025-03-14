import { Bid } from "@/types";
import { create } from "zustand";

type State = {
    bids: Bid[];
    open: boolean;
};

type Actions = {
    setBids: (bids: Bid[]) => void;
    addBid: (bid: Bid) => void;
    setOpen: (value: boolean) => void;
};

export const useBidStore = create<State & Actions>((set) => ({
    bids: [],
    //this is the initial fetch of bids
    open: true,
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
    setOpen: (value: boolean) => {
        set(() => ({
            open: value,
        }));
    },
}));
