//this is from the documentation

export { auth as middleware } from "@/auth";

export const config = {
    matcher: [
        "/session", //user cant access session if not authenticated
    ],
    pages: {
        //route to custom signin page
        signIn: "/api/auth/signin",
    },
};
