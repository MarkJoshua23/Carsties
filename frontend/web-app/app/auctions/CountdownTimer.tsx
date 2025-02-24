//client because raect countdown uses usestate, etc
'use client'

import React from 'react'
import Countdown from 'react-countdown'

type Time = {
    days: number
    hours: number
    minutes: number
    seconds: number
    completed: boolean
}

type Props ={
    auctionEnd: string
}

// Renderer callback with condition
const renderer = ({ days, hours, minutes, seconds, completed }:Time) => {
    if (completed) {
      // Render a completed state
      return <span>Finished</span>;
    } else {
      // Render a countdown
      return <span>{days}:{hours}:{minutes}:{seconds}</span>;
    }
  };
export default function CountdownTimer({auctionEnd}:Props) {
  return (
    <div>
        <Countdown
        date={auctionEnd}
        renderer={renderer}
        />
    </div>
  )
}
