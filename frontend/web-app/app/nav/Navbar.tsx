import React from "react";
import Search from "./Search";
import Logo from "./Logo";
import LoginButton from "./LoginButton";
import { getCurrentUser } from "../actions/authActions";
import UserActions from "./UserActions";

export default async function Navbar() {
    //check if the user is logged in
    const user = await getCurrentUser();
    return (
        //display session only when
        <header className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
            <Logo />
            <Search />
            {user ? <UserActions user={user} /> : <LoginButton />}
        </header>
    );
}
