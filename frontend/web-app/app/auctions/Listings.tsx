import React from 'react'
import AuctionCard from './AuctionCard';

//this will also cache
async function getData() {
    const res = await fetch('http://localhost:6001/search?pageSize=10',{
        next: { revalidate: 60 } // Cache for 60 seconds, then refresh
    });
    if (!res.ok) throw new Error('Failed to fetch data');
    //get the respose from res
    return res.json();
}

export default async function Listings() {
    //store the fetched data to data
    const data = await getData();

  return (
    <div className='grid lg:grid-cols-4 sm:grid-cols-2 gap-6'>
        {data && data.results.map((auction:any, index:any)=>(
          <AuctionCard auction={auction} key={index}/>
        ))}
    </div>
  )
}
