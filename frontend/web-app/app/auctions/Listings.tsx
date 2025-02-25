"use client";
import React, { useEffect, useState } from "react";
import AuctionCard from "./AuctionCard";
import { Auction, PagedResult } from "@/types";
import AppPagination from "../components/AppPagination";
import { getData } from "../actions/auctionActions";
import { Spinner } from "flowbite-react";
import Filters from "./Filters";

export default function Listings() {
  //store the fetched data to data
  // const data = await getData();
  //array of Auction type
  const [auctions, setAuctions] = useState<Auction[]>([]);
  //total page count so pagination can render the proper numbers
  const [pageCount, setPageCount] = useState(0);
  //in which page are we currently
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(4);

  useEffect(() => {
    getData(pageNumber, pageSize).then((data) => {
      setAuctions(data.results);
      setPageCount(data.pageCount);
    });
    //call useeffect everytime pagenumber, and pagesize changes
  }, [pageNumber, pageSize]);

  if (auctions.length === 0)
    return <Spinner aria-label="Extra large spinner example" size="xl" />;

  return (
    <>
      <Filters pageSize={pageSize} setPageSize={setPageSize} />
      <div className="grid lg:grid-cols-4 sm:grid-cols-2 gap-6">
        {auctions.map((auction, index) => (
          <AuctionCard auction={auction} key={index} />
        ))}
      </div>
      <div className="flex justify-center mt-4 ">
        <AppPagination
          currentPage={pageNumber}
          pageCount={pageCount}
          //pass the usestate function
          pageChanged={setPageNumber}
        />
      </div>
    </>
  );
}
