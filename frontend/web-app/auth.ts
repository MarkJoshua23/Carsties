import NextAuth, { Profile } from "next-auth";
import { OIDCConfig } from "next-auth/providers";
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6";

export const { handlers, signIn, signOut, auth } = NextAuth({
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
      //get claims automatically
      idToken: true,
    } as OIDCConfig<Profile>),
  ],
});
