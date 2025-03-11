using System;
using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly GrpcAuctionClient _grpcClient;

    public BidsController(IMapper mapper, IPublishEndpoint publishEndpoint, GrpcAuctionClient grpcClient)
    {
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _grpcClient = grpcClient;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BidDTO>> PlaceBid(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);

        //check if the auction item exists
        if (auction == null)
        {
            //try to fetch the auction from auction service if it exist
            auction = _grpcClient.GetAuction(auctionId);

            if (auction == null) return BadRequest("Cannot accept bids on this auction");
        }

        //check if its the user's own item, user cant bid to their items
        if (auction.Seller == User.Identity.Name)
        {
            return BadRequest("You cannot bid on your own auction");
        }

        //ready to insert a new bid
        var bid = new Bid
        {
            Amount = amount,
            AuctionId = auctionId,
            Bidder = User.Identity.Name
        };

        //if auctionend is less than, that means it is already finished
        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            //we'll not register it but still save it as record
            bid.BidStatus = BidStatus.Finished;
        }
        else
        {
            //get the existing highest bid
            var highBid = await DB.Find<Bid>().
            // 'a' is the context of DB
            Match(a => a.AuctionId == auctionId)
            .Sort(b => b.Descending(x => x.Amount))
            .ExecuteFirstAsync();

            //check if the users bid is higher than the highest bid in the db
            //also true if theres no bid 
            if (highBid != null && amount > highBid.Amount || highBid == null)
            {

                //accepted if its higher than the reserveprice 
                //also accepted if its below as long but is given with different status
                bid.BidStatus = amount > auction.ReservePrice
                ? BidStatus.Accepted
                : BidStatus.AcceptedBelowReserve;
            }

            //if the bid is less than the highest recorded bid
            if (highBid != null && bid.Amount <= highBid.Amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }

        await DB.SaveAsync(bid);

        //publish to rabbitmq so the other service know if new bids are placed
        await _publishEndpoint.Publish(_mapper.Map<BidPlaced>(bid));
        return Ok(_mapper.Map<BidDTO>(bid));
    }

    //get bids of a particular auction item
    [HttpGet("{auctionId}")]
    public async Task<ActionResult<List<BidDTO>>> GetBidsForAuction(string auctionId)
    {
        var bids = await DB.Find<Bid>()
        .Match(a => a.AuctionId == auctionId)
        .Sort(b => b.Descending(a => a.BidTime))
        .ExecuteAsync();

        return bids.Select(_mapper.Map<BidDTO>).ToList();
    }
}