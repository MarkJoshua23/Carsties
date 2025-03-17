import type { NextConfig } from "next";

const nextConfig: NextConfig = {
    logging: {
        fetches: {
            fullUrl: true,
        },
    },
    //config to allow images from url
    images: {
        remotePatterns: [{ protocol: "https", hostname: "cdn.pixabay.com" }],
    },
    eslint: {
        ignoreDuringBuilds: true, // Allow production builds even with ESLint errors
    },
    output: "standalone",
};

export default nextConfig;
