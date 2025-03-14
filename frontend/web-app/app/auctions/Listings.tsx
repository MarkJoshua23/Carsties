"use client";
import React, { useEffect, useState } from "react";
import AuctionCard from "./AuctionCard";
import { Auction, PagedResult } from "@/types";
import AppPagination from "../components/AppPagination";
import { getData } from "../actions/auctionActions";
import { Spinner } from "flowbite-react";
import Filters from "./Filters";
import { useParamsStore } from "@/hooks/useParamsStore";
import { shallow, useShallow } from "zustand/shallow";
import qs from "query-string";
import EmptyFilter from "../components/EmptyFilter";
import { useAuctionStore } from "@/hooks/useAuctionStore";

export default function Listings() {
    // //store the fetched data to data
    // // const data = await getData();
    // //array of Auction type
    // const [auctions, setAuctions] = useState<Auction[]>([]);
    // //total page count so pagination can render the proper numbers
    // const [pageCount, setPageCount] = useState(0);
    // //in which page are we currently
    // const [pageNumber, setPageNumber] = useState(1);
    // const [pageSize, setPageSize] = useState(4);

    const [loading, setLoading] = useState(true);
    //get the specific params from zustand state
    //shallow ensures re-renders only happen when actual values change.
    const params = useParamsStore(
        useShallow((state) => ({
            pageNumber: state.pageNumber,
            pageSize: state.pageSize,
            searchTerm: state.searchTerm,
            orderBy: state.orderBy,
            filterBy: state.filterBy,
            seller: state.seller,
            winner: state.winner,
        }))
    );

    //store the data from api
    const data = useAuctionStore(
        useShallow((state) => ({
            auctions: state.auctions,
            totalCount: state.totalCount,
            pageCount: state.pageCount,
        }))
    );

    const setData = useAuctionStore((state) => state.setData);
    const setParams = useParamsStore((state) => state.setParams);
    //convert params object to url like // "https://example.com?pageNumber=1&pageSize=10&searchTerm=react"
    const url = qs.stringifyUrl({ url: "", query: params });

    function setPageNumber(pageNumber: number) {
        //use zustand to alter the pagenumber
        setParams({ pageNumber });
    }

    //this will run everytime theres a change in state
    useEffect(() => {
        //pass the url so it can get the data
        getData(url).then((data) => {
            setData(data);
            setLoading(false);
        });
        //call useeffect everytime url (pagenumber, pagesize, searchterm) changes
    }, [url]);

    if (loading)
        return <Spinner aria-label="Extra large spinner example" size="xl" />;

    return (
        <>
            <Filters />
            {data.totalCount === 0 ? (
                <EmptyFilter showReset />
            ) : (
                <>
                    <div className="grid lg:grid-cols-4 sm:grid-cols-2 gap-6">
                        {data.auctions.map((auction, index) => (
                            <AuctionCard auction={auction} key={index} />
                        ))}
                    </div>
                    <div className="flex justify-center mt-4 ">
                        <AppPagination
                            currentPage={params.pageNumber}
                            pageCount={data.pageCount}
                            //pass the usestate function
                            pageChanged={setPageNumber}
                        />
                    </div>
                </>
            )}
        </>
    );
}
