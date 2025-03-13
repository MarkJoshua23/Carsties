"use client";
import { getBidsForAuction } from "@/app/actions/auctionActions";
import Heading from "@/app/components/Heading";
import { useBidStore } from "@/hooks/useBidStore";
import { Auction, Bid } from "@/types";
import { error } from "console";
import { Spinner } from "flowbite-react";
import { User } from "next-auth";
import React, { useEffect, useState } from "react";
import toast from "react-hot-toast";
import BidItem from "./BidItem";

type Props = {
    user: User | null;
    auction: Auction;
};

export default function BidList({ user, auction }: Props) {
    const [loading, setLoading] = useState(true);
    const bids = useBidStore((state) => state.bids);
    const setBids = useBidStore((state) => state.setBids);

    useEffect(() => {
        getBidsForAuction(auction.id)
            .then((res: any) => {
                if (res.error) {
                    throw res.error;
                }
                //give types of Bid to res
                setBids(res as Bid[]);
            })
            .catch((err) => {
                toast.error(err.message);
            })
            .finally(() => setLoading(false));
    }, [auction.id, setLoading, setBids]);

    if (loading)
        return <Spinner aria-label="Extra large spinner example" size="xl" />;

    return (
        <div className="border-2 rounded-lg p-2 bg-gray-100">
            <Heading title="Bids" />
            {bids.map((bid) => (
                <BidItem key={bid.id} bid={bid} />
            ))}
        </div>
    );
}
