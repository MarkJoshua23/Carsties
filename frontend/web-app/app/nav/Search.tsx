"use client";
import { useParamsStore } from "@/hooks/useParamsStore";
import { usePathname, useRouter } from "next/navigation";
import React, { useState } from "react";
import { FaSearch } from "react-icons/fa";

export default function Search() {
    const router = useRouter();
    const pathName = usePathname();
    const setParams = useParamsStore((state) => state.setParams);
    //this will be for omchange to update the values
    const setSearchValue = useParamsStore((state) => state.setSearchValue);
    const searchValue = useParamsStore((state) => state.searchValue);

    function onChange(e: any) {
        setSearchValue(e.target.value);
    }

    //this will trigger re render
    //the getdata will get new data with searchterm since the useffect listens for change in url
    function search() {
        //redirect to home if searching outside
        if (pathName !== "/") router.push("/");
        setParams({ searchTerm: searchValue });
    }
    return (
        <div className="flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm">
            <input
                onKeyDown={(e: any) => {
                    if (e.key === "Enter") search();
                }}
                value={searchValue}
                onChange={onChange}
                type="text"
                placeholder="Search for cars by make, model, or color"
                //flex-grow will grow the input based on available space
                className="flex-grow pl-5 bg-transparent focus:outline-none border-transparent focus:border-transparent focus:ring-0 text-sm text-gray-600"
            />
            <button onClick={search}>
                <FaSearch
                    size={34}
                    className="bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2"
                />
            </button>
        </div>
    );
}
