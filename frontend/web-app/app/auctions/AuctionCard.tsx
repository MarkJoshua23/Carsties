import Image from 'next/image'
import React from 'react'
import CountdownTimer from './CountdownTimer'

type Props ={
    auction: any
}
//image fill will fill up the container with the image
//object cover makes the image cover the aspect ratio and not stretch
export default function AuctionCard({auction} : Props) {
  //relative so the image will base its scale on the div
  //countdown is absolute so it can be stacked above, since theres no available space bc the image filled all space
  return (
    <a href="#">
      
      <div className="relative w-full bg-gray-200 aspect-[16/10] rounded-lg overflow-hidden">
        <Image
        src={auction.imageUrl}
        alt='image'
        fill
        priority
        className='object-cover'
        sizes='(max-width: 758px) 100vw, (max-width:1200px) 50vw , 25vw'
        />
        <div className='absolute bottom-2 left-2'>
        <CountdownTimer auctionEnd={auction.auctionEnd}/>
        </div>
      </div>
      <div className="flex justify-between items-center mt-4">
        <h3 className='text-gray-700 '>
          {auction.make} {auction.model}
        </h3>
        <p className='font-semibold text-sm'>
          {auction.year}
        </p>
      </div>
      
    </a>
  )
}
