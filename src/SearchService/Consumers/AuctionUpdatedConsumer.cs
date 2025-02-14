using System;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--------->Consuming auction updated" + context.Message.Id);
        // put auctionupdated in a local var
        var item = context.Message;

        // Create an Item object with just the properties we want to update
        var updateData = new Item
        {
            Make = item.Make,
            Model = item.Model,
            Year = item.Year,
            Color = item.Color,
            MileAge = item.MileAge
        };

        await DB.Update<Item>()
        .MatchID(item.Id)
        .ModifyOnly(m => new { m.Make, m.Model, m.Year, m.Color, m.MileAge }, updateData)
        .ExecuteAsync();

    }
}
