using System;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;
// click lightbulb and "implement interface"
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    //automapper dependency injection
    //lightbuld then create _mapper
    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    //CREATE
    //consume the auctioncreated
    //dont forget to async
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("--------->Consuming auction created" + context.Message.Id);
        // put the consumed items in Item =
        var item = _mapper.Map<Item>(context.Message);

        //argument exception means the argument/parameter passed is invalid
        if (item.Model == "foo") throw new ArgumentException("Cannot sell cars named Foo");
        //add to mongo
        await item.SaveAsync();
    }
}
