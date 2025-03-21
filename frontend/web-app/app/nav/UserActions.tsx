"use client";
import { useParamsStore } from "@/hooks/useParamsStore";
import {
    Button,
    Dropdown,
    DropdownDivider,
    DropdownItem,
} from "flowbite-react";
import { User } from "next-auth";
import { signOut } from "next-auth/react";
import Link from "next/link";
//router from navigaton is recommended for newer next projects
import { usePathname, useRouter } from "next/navigation";
import React from "react";
import { AiFillCar, AiFillTrophy, AiOutlineLogout } from "react-icons/ai";
import { HiCog, HiUser } from "react-icons/hi";

type Props = {
    user: User;
};
export default function UserActions({ user }: Props) {
    const router = useRouter();
    const pathName = usePathname();
    const setParams = useParamsStore((state) => state.setParams);

    //this sets the filter to show items that the user is the winner which will trigger re fetch because of the useaeffect
    function setWinner() {
        setParams({ winner: user.username, seller: undefined });
        if (pathName !== "/") router.push("/");
    }
    //display items where user is the seller
    function setSeller() {
        setParams({ seller: user.username, winner: undefined });
        if (pathName !== "/") router.push("/");
    }
    return (
        <Dropdown inline label={`Welcome ${user.name}`}>
            <DropdownItem icon={HiUser} onClick={setSeller}>
                My Auctions
            </DropdownItem>
            <DropdownItem icon={AiFillTrophy} onClick={setWinner}>
                Auctions won
            </DropdownItem>
            <DropdownItem icon={AiFillCar}>
                <Link href="/auctions/create">Sell my car</Link>
            </DropdownItem>
            <DropdownItem icon={HiCog}>
                <Link href="/session">Session (dev only) </Link>
            </DropdownItem>
            <DropdownDivider />
            <DropdownItem
                icon={AiOutlineLogout}
                onClick={() => signOut({ redirectTo: "/" })}
            ></DropdownItem>
        </Dropdown>
    );
}
