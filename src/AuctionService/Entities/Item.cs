using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionService.Entities;
//Primary Key = Id
//Foreign Key = <navproperty>+Id

//specify the tablename
[Table("Items")]
public class Item
{
    public Guid Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int MileAge { get; set; }
    public string ImageUrl { get; set; }

    //nav property to know what class is related

    public Auction Auction { get; set; }
    //convention <navproperty>+Id = AuctionId, it knows the Id in Auction is related

    public Guid AuctionId { get; set; }

}
