import type { Metadata } from "next";
//root for route '/'
import "./globals.css";
import Navbar from "./nav/Navbar";
import ToasterProvider from "./providers/ToasterProvider";

export const metadata: Metadata = {
    title: "Carsties",
    description: "Generated by create next app",
};

export default function RootLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    //children in main is the page tsx
    return (
        <html lang="en">
            <body>
                <ToasterProvider />
                <Navbar />
                <main className="container mx-auto px-5 pt-10">{children}</main>
            </body>
        </html>
    );
}
