using System;

namespace Contracts;

public class AuctionCreated
{
    public Guid Id { get; set; }
    //=0 to put default value
    public int ReservePrice { get; set; } = 0;
    public string Seller { get; set; }
    public string Winner { get; set; }
    public int SoldAmount { get; set; }
    public int CurrentHighBid { get; set; }
    //UTC so its universal
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime AuctionEnd { get; set; }
    //enum
    public string Status { get; set; }

    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int MileAge { get; set; }
    public string ImageUrl { get; set; }


}
