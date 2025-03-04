"use server";
//this will check if theres user
import { auth } from "@/auth";

export async function getCurrentUser() {
    try {
        //use useSession if its a client function and not server
        const session = await auth();
        if (!session) return null;
        return session.user;
    } catch (error) {
        return null;
    }
}
