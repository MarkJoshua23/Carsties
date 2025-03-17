import { url } from "inspector";
import NextAuth, { Profile } from "next-auth";
import { OIDCConfig } from "next-auth/providers";
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6";

//copy this from auth js documentation
export const { handlers, signIn, signOut, auth } = NextAuth({
    //the info the frontend can access
    session: {
        //where is the user information located
        //jwt because the user info/claims are already there
        strategy: "jwt",
    },
    providers: [
        DuendeIDS6Provider({
            id: "id-server",
            //should be same as the identity client in .net
            clientId: "nextApp",
            clientSecret: "secret",
            //url of identity server
            //issuer should be the same as the issuer in the identity server
            issuer: process.env.ID_URL,
            authorization: {
                params: { scope: "openid profile auctionApp" },
                url: process.env.ID_URL + "/connect/authorize",
            },
            token: {
                url: `${process.env.ID_URL_INTERNAL}/connect/token`,
            },
            userinfo: {
                url: `${process.env.ID_URL_INTERNAL}/connect/token`,
            },
            //get claims automatically, request for jwt
            idToken: true,
            //Omit ignores the username that is originally used for token since it will not be used in this provider
        } as OIDCConfig<Omit<Profile, "username">>),
    ],
    //to see jwt infos
    callbacks: {
        async redirect({ url, baseUrl }) {
            return url.startsWith(baseUrl) ? url : baseUrl;
        },
        //to enable secured routes
        async authorized({ auth }) {
            return !!auth;
        },

        async jwt({ token, profile, account }) {
            console.log({ token, profile });
            //put access token from account to token to use that to auth to api
            if (account && account.access_token) {
                token.accessToken = account.access_token;
            }

            //put username from profile to token
            if (profile) {
                token.username = profile.username;
            }
            return token;
        },

        async session({ session, token }) {
            //accesstoken from token to session so frontend can send req to apis
            //username from token to session so frontend can use it
            if (token) {
                session.user.username = token.username;
                session.accessToken = token.accessToken;
            }
            return session;
        },
    },
});
