using System;
using Microsoft.AspNetCore.Http.Features;

namespace AuctionService.Entities;

public class Auction
{
    //Guid is like uid
    // naming a property Id or anythingId makes it PK
    public Guid Id { get; set; }
    //=0 to put default value
    public int ReservePrice { get; set; } = 0;
    public string Seller { get; set; }
    public string Winner { get; set; }
    public int? SoldAmount { get; set; }
    public int? CurrentHighBid { get; set; }
    //UTC so its universal
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime AuctionEnd { get; set; }
    //enum
    public Status Status { get; set; }
    //this is added just to make it know its relate to Item class
    public Item Item { get; set; }
}
