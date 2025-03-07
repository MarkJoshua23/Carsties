"use client";
import { deleteAuction } from "@/app/actions/auctionActions";
import { error } from "console";
import { Button } from "flowbite-react";
import { usePathname, useRouter } from "next/navigation";
import React, { useState } from "react";
import toast from "react-hot-toast";

type Props = {
    id: string;
};
export default function DeleteButton({ id }: Props) {
    const [loading, setLoading] = useState(false);
    const router = useRouter();
    const pathName = usePathname();
    function doDelete() {
        setLoading(true);
        //the result is passed to then 'res'
        deleteAuction(id)
            .then((res) => {
                console.log(res);
                console.log(pathName);
                if (res.error) throw res.error;
                toast.success("Successfully deleted an item");
                router.push("/");
            })
            .catch((error) => {
                toast.error(error.status + " " + error.message);
            })
            .finally(() => setLoading(false));
    }
    return (
        <Button color="failure" isProcessing={loading} onClick={doDelete}>
            Delete Auction
        </Button>
    );
}
