"use client";
import { useAuctionStore } from "@/hooks/useAuctionStore";
import { useBidStore } from "@/hooks/useBidStore";
import { Bid } from "@/types";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { useParams } from "next/navigation";
import React, { ReactNode, useCallback, useEffect, useRef } from "react";

type Props = {
    children: ReactNode;
};
export default function SignalRProvider({ children }: Props) {
    //just like usestate but no rerender
    const connection = useRef<HubConnection | null>(null);
    //to change the price of the price in the homepage
    const setCurrentPrice = useAuctionStore((state) => state.setCurrentPrice);
    const addBid = useBidStore((state) => state.addBid);
    const params = useParams<{ id: string }>();

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
        return () => {
            connection.current?.off("BidPlaced", handleBidPlaced);
        };
    }, [setCurrentPrice, handleBidPlaced]);
    //children is the component inside this component when called
    return children;
}
