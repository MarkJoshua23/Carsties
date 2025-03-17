import {
    getBidsForAuction,
    getDetailedViewData,
} from "@/app/actions/auctionActions";
import Heading from "@/app/components/Heading";
import React from "react";
import CountdownTimer from "../../CountdownTimer";
import CardImage from "../../CardImage";
import DetailedSpecs from "./DetailedSpecs";
import EditButton from "./EditButton";
import { getCurrentUser } from "@/app/actions/authActions";
import DeleteButton from "./DeleteButton";
import BidItem from "./BidItem";
import BidList from "./BidList";

export default async function Details({
    params,
}: {
    params: Promise<{ id: string }>;
}) {
    const id = (await params).id;
    //the specific auction
    const data = await getDetailedViewData(id);
    const user = await getCurrentUser();
    const bids = await getBidsForAuction(id);
    return (
        <div className="">
            <div className="flex justify-between">
                <div className="flex items-center gap-3">
                    <Heading title={`${data.make} ${data.model}`} />
                    {user?.username === data.seller && (
                        <>
                            <EditButton id={data.id} />
                            <DeleteButton id={data.id} />
                        </>
                    )}
                </div>
                <div className="flex gap-3">
                    <h3 className="text-2xl font-semibold">Time remaining: </h3>
                    <CountdownTimer auctionEnd={data.auctionEnd} />
                </div>
            </div>
            <div className="grid grid-cols-2 gap-6 mt-3">
                <div className="w-full bg-gray-200 relative aspect-[4/3] rounded-lg overflow-hidden">
                    <CardImage imageUrl={data.imageUrl} />
                </div>
                <BidList user={user} auction={data} />
            </div>
            <div className="mt-3 grid grid-cols-1 rounded-lg">
                <DetailedSpecs auction={data} />
            </div>
        </div>
    );
}
