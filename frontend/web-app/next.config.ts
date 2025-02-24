import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  logging:{
    fetches:{
      fullUrl:true
    }
  },
  //config to allow images from url
  images:{
    remotePatterns:[
      {protocol: 'https', hostname: "cdn.pixabay.com"}
    ]
  }
};

export default nextConfig;
