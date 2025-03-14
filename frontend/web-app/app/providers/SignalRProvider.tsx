"use client";
import { useAuctionStore } from "@/hooks/useAuctionStore";
import { useBidStore } from "@/hooks/useBidStore";
import { Auction, AuctionFinished, Bid } from "@/types";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { User } from "next-auth";
import { useParams } from "next/navigation";
import React, { ReactNode, useCallback, useEffect, useRef } from "react";
import AuctionCreatedToast from "../components/AuctionCreatedToast";
import toast from "react-hot-toast";
import { getDetailedViewData } from "../actions/auctionActions";
import AuctionFinishedToast from "../components/AuctionFinishedToast";

type Props = {
    children: ReactNode;
    user: User | null;
};
export default function SignalRProvider({ children, user }: Props) {
    //just like usestate but no rerender
    const connection = useRef<HubConnection | null>(null);
    //to change the price of the price in the homepage
    const setCurrentPrice = useAuctionStore((state) => state.setCurrentPrice);
    const addBid = useBidStore((state) => state.addBid);
    const params = useParams<{ id: string }>();

    //toast that appears if someone auctions
    const handleAuctionCreated = useCallback(
        (auction: Auction) => {
            //toast only appears if someone other than the user auctioned a car
            if (user?.username !== auction.seller) {
                return toast(<AuctionCreatedToast auction={auction} />, {
                    duration: 10000,
                });
            }
        },
        [user?.username]
    );

    const handleAuctionFinished = useCallback(
        (finishedAuction: AuctionFinished) => {
            //dont use await if we want to use the promise for loading
            const auction = getDetailedViewData(finishedAuction.auctionId);
            return toast.promise(
                auction,
                {
                    loading: "Loading",
                    success: (auction) => (
                        <AuctionFinishedToast
                            auction={auction}
                            finishedAuction={finishedAuction}
                        />
                    ),
                    error: (err) => "Auction finished",
                },
                { success: { duration: 10000, icon: null } }
            );
        },
        []
    );

    //so the function will not rebuilld every rerender
    const handleBidPlaced = useCallback(
        (bid: Bid) => {
            //if the bid is accepted then set the highest bid
            if (bid.bidStatus.includes("Accepted")) {
                setCurrentPrice(bid.auctionId, bid.amount);
            }
            //add it if the user is in the details page of the specific car for bid
            if (params.id === bid.auctionId) {
                addBid(bid);
            }
            //only updates when something in [] updates
        },
        [setCurrentPrice, addBid]
    );
    useEffect(() => {
        //if we dont have a connection then connect to signalr
        if (!connection.current) {
            //update the connection useref
            connection.current = new HubConnectionBuilder()
                //gateway url
                .withUrl("http://localhost:6001/notifications")
                .withAutomaticReconnect()
                .build();
            connection.current
                .start()
                .then(() => "Connected to SignalR")
                .catch((err) => console.log(err));
        }

        //to listen to a particular method name
        //use the methodname in the signalr
        //argument1= method, argument2=returned values
        connection.current.on("BidPlaced", handleBidPlaced);

        //listen to the signalr auctioncreated
        connection.current.on("AuctionCreated", handleAuctionCreated);
        connection.current.on("AuctionFinished", handleAuctionFinished);

        return () => {
            connection.current?.off("BidPlaced", handleBidPlaced);
            connection.current?.off("AuctionCreated", handleAuctionCreated);
            connection.current?.off("AuctionFinished", handleAuctionFinished);
        };
    }, [setCurrentPrice, handleBidPlaced, handleAuctionFinished]);
    //children is the component inside this component when called
    return children;
}
