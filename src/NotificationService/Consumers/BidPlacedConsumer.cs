using System;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly IHubContext<NotificationHub> _hubContext;
    //inject hub then lightblb=>assign field
    public BidPlacedConsumer(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("====>Bid placed message recieved");

        //this will send notification to the connected client if the consumer is triggered
        await _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}
