using System;
using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _dbcontext;

    //inject to constructor
    public AuctionFinishedConsumer(AuctionDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }


    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine("---> Consuming Auction Finished");

        //look for db item that matches the id of auction item that is finished
        var auction = await _dbcontext.Auctions.FindAsync(context.Message.AuctionId);

        //if that item is sold=true
        if (context.Message.ItemSold)
        {
            //update the winner and the amount 
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }
        //reserved price= minimum acceptable price for the seller 
        auction.Status = auction.SoldAmount > auction.ReservePrice
        ? Status.Finished : Status.ReserveNotMet;

        await _dbcontext.SaveChangesAsync();
    }
}
