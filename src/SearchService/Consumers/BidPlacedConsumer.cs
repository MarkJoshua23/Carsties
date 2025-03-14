using System;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("---> Consuming Bid Placed");
        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

        //this will make sure that the currenthighbid stays the highest number
        if (context.Message.BidStatus.Contains("Accepted") &&
        context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auction.SaveAsync();
        }
    }
}
