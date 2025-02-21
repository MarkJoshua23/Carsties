import React from 'react'

//this will also cache
async function getData() {
    const res = await fetch('http://localhost:6001/search',{
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
    <div>

        {JSON.stringify(data, null , 2)}
    </div>
  )
}
