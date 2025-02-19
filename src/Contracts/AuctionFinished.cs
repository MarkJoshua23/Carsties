using System;

namespace Contracts;

public class AuctionFinished
{
    //is the bid success and the item is sold
    public bool ItemSold { get; set; }
    public string AuctionId { get; set; }
    public string Winner { get; set; }
    public string Seller { get; set; }
    public int? Amount { get; set; }
}
