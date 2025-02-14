using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Models;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// httpclient and the polling
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

//rabbitmq
builder.Services.AddMassTransit(x =>
{
    //make consumer available
    //this will know that this consumer should look for 
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    //to name the consumer search-AuctionCreatedConsumer so there will be no conflict if other service use the same consumer name
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, cfg) =>
    {
        //configure search-auction-created
        //retry the consumer if theres problem like if mongodb doesnt work
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            //5 times every 5 secs
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("search-auction-updated", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("search-auction-deleted", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionDeletedConsumer>(context);
        });
        //rest of endpoints
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

//lifetime is important so it still call app.run even if the try catch is failing to connect to the auction service
app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        //repeat until theres connection bc of polling
        await DbInitializer.InitDb(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});



app.Run();
//polling will repeat request of http errors
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
=> HttpPolicyExtensions
.HandleTransientHttpError()
.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
//try every 3 seconds
.WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));