using System;
using BiddingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Bid>> PlaceBid(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);

        //check if the auction item exists
        if (auction == null)
        {
            //TODO check with auction service 
            return NotFound();
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
        return Ok(bid);
    }

    //get bids of a particular auction item
    [HttpGet("{auctionId}")]
    public async Task<ActionResult<List<Bid>>> GetBidsForAuction(string auctionId)
    {
        var bids = await DB.Find<Bid>()
        .Match(a => a.AuctionId == auctionId)
        .Sort(b => b.Descending(a => a.BidTime))
        .ExecuteAsync();

        return bids;
    }
}