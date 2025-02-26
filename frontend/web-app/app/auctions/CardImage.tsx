"use client";
import Image from "next/image";
import React, { useState } from "react";

type CarImage = {
  imageUrl: string;
};
export default function ({ imageUrl }: CarImage) {
  //group hover can be used if parent have group css and it activates when hovered
  const [isLoading, setIsLoading] = useState(true);
  return (
    <Image
      src={imageUrl}
      alt="image"
      fill
      priority
      //so the image will be lighter when hovered
      className={`object-cover group-hover:opacity-75 duration-300 ease-in-out
                ${
                  isLoading
                    ? "grayscale blur-2xl scale-110"
                    : "grayscale-0 blur-0 scale-100"
                }
                `}
      sizes="(max-width: 758px) 100vw, (max-width:1200px) 50vw , 25vw"
      onLoad={() => setIsLoading(false)}
    />
  );
}
