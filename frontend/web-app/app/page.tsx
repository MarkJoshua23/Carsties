import Image from "next/image";
import Listings from "./nav/Listings";

export default function Home() {
  console.log("server");

  return (
    <div className="">
      <Listings />
    </div>
  );
}
