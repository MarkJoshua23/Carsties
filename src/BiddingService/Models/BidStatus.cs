namespace BiddingService.Models;

public enum BidStatus
{
    Accepted,
    //accepted but still below the reserveprice(minimum price the seller wanted)
    AcceptedBelowReserve,
    TooLow,
    Finished

}
