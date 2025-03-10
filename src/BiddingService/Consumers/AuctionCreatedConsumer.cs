using System;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Consumers;
//lightbulb => interface
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    //this will listen to rabbitmq if theres  a change in AuctionCreated
    public Task Consume(ConsumeContext<AuctionCreated> context)
    {
        var auction = new Auction
        {
            ID = context.Message.Id.ToString(),
            Seller = context.Message.Seller,
            AuctionEnd = context.Message.AuctionEnd,
            ReservePrice = context.Message.ReservePrice,
        };

        return auction.SaveAsync();
    }
}