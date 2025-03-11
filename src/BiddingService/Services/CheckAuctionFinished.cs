using System;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Services;

public class CheckAuctionFinished : BackgroundService
{
    private readonly ILogger<CheckAuctionFinished> _logger;
    private readonly IServiceProvider _services;

    public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting check for finished auction");
        stoppingToken.Register(() => _logger.LogInformation("===> Auction check is stopping"));

        //run this as long as stopping token is not called
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuctions(stoppingToken);
            //run task every 5 secs
            await Task.Delay(5000, stoppingToken);
        }
    }

    //assigns auction items as 'finished' and the final amount
    private async Task CheckAuctions(CancellationToken stoppingToken)
    {
        var finishedAuctions = await DB.Find<Auction>()
        // items that is ended
        .Match(x => x.AuctionEnd <= DateTime.UtcNow)
        //items not finished, since we dont want to modify the finshed(items that already went to this service)
        .Match(x => !x.Finished)
        //to stop the query
        .ExecuteAsync(stoppingToken);

        if (finishedAuctions.Count == 0) return;
        _logger.LogInformation($"===> Found {finishedAuctions.Count} auctions that have completed");

        using var scope = _services.CreateScope();
        var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        foreach (var auction in finishedAuctions)
        {
            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);
            var winningBid = await DB.Find<Bid>()
            .Match(a => a.AuctionId == auction.ID)
            //filters to only bids higher than the reserved price
            .Match(b => b.BidStatus == BidStatus.Accepted)
            .Sort(x => x.Descending(s => s.Amount))
            .ExecuteFirstAsync(stoppingToken);

            await endpoint.Publish(new AuctionFinished
            {
                ItemSold = winningBid != null,
                AuctionId = auction.ID,
                //? so there can be no winner(make it optional)
                Winner = winningBid?.Bidder,
                Amount = winningBid?.Amount,
                Seller = auction.Seller,
            }, stoppingToken);
        }

    }
}
