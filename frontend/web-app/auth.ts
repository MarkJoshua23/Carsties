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
            issuer: "http://localhost:5000",
            authorization: { params: { scope: "openid profile auctionApp" } },
            //get claims automatically, request for jwt
            idToken: true,
            //Omit ignores the username that is originally used for token since it will not be used in this provider
        } as OIDCConfig<Omit<Profile, "username">>),
    ],
    //to see jwt infos
    callbacks: {
        //to enable secured routes
        async authorized({ auth }) {
            return !!auth;
        },
        //put username from profile to token
        async jwt({ token, profile }) {
            console.log({ token, profile });

            if (profile) {
                token.username = profile.username;
            }
            return token;
        },

        //username from token to session so frontend can use it
        async session({ session, token }) {
            if (token) {
                session.user.username = token.username;
            }
            return session;
        },
    },
});
