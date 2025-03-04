import NextAuth, { type DefaultSession } from "next-auth";
import { JWT } from "next-auth/jwt";

declare module "next-auth" {
    interface Session {
        user: {
            //add custom session value to user so u have session.user.username
            username: string;
        } & DefaultSession["user"];
    }
    interface Profile {
        username: string;
    }
}

//fix types in jwt
//add custom type username in jwt values
declare module "next-auth/jwt" {
    interface JWT {
        username: string;
    }
}
