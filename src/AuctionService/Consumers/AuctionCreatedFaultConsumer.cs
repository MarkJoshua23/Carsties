using System;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AuctionService.Consumers;
//this will consume the fault of the consumer services
public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("---> Consuming Faulty creation");
        var exception = context.Message.Exceptions.First();
        //check what kind of exception, argument exception means the argument/parameter passed is invalid
        if (exception.ExceptionType == "System.ArgumentException")
        {
            //put the fixes for exception here

            //for example change the model name si its not foo
            context.Message.Message.Model = "Foobar";

            //publish the fixed item
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine("Not a argument error");
        }
    }
}
