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
import { numberWithCommas } from "@/app/lib/numberWithComma";
import EmptyFilter from "@/app/components/EmptyFilter";

type Props = {
    user: User | null;
    auction: Auction;
};

export default function BidList({ user, auction }: Props) {
    const [loading, setLoading] = useState(true);
    const bids = useBidStore((state) => state.bids);
    const setBids = useBidStore((state) => state.setBids);
    //loops all the bids to find the highest
    const highBid = bids.reduce(
        (prev, current) => (prev > current.amount ? prev : current.amount),
        0
    );

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
        //overflow auto makes it scrollable
        <div className="rounded-lg shadow-md">
            <div className="py-2 px-4 bg-white">
                <div className="sticky top-0 bg-white p-2">
                    <Heading
                        title={`Current high bid is ${numberWithCommas(
                            highBid
                        )}`}
                    />
                </div>
            </div>
            <div className="overflow-auto h-[400px] flex-col-reverse px-2">
                {bids.length === 0 ? (
                    <EmptyFilter
                        title="No bids for this item"
                        subtitle="Please feel free to make a bid"
                    />
                ) : (
                    <>
                        {bids.map((bid) => (
                            <BidItem key={bid.id} bid={bid} />
                        ))}
                    </>
                )}
            </div>
        </div>
    );
}
