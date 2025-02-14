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
        // map the contract to local entity, dont worry about the null
        var item = _mapper.Map<Item>(context.Message);

        var result = await DB.Update<Item>()
        .MatchID(context.Message.Id)
        .ModifyOnly(m => new { m.Make, m.Model, m.Year, m.Color, m.MileAge }, item)
        .ExecuteAsync();

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb");
        }

    }
}
