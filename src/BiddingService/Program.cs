using BiddingService.Consumers;
using BiddingService.Services;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        //define the host
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            //define the username, if no env var then default to "guest"
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});
//authentcation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //put the authority ro the identity server we made
    //ASP.NET sends the token to IdentityServer to verify if it’s valid
    options.Authority = builder.Configuration["IdentityServiceUrl"];
    //to allow http
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;

    // Maps the 'username' claim to User.Identity.Name
    options.TokenValidationParameters.NameClaimType = "username";
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHostedService<CheckAuctionFinished>();
var app = builder.Build();



app.UseAuthorization();

app.MapControllers();

await DB.InitAsync("BidDb", MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("BidBdConnection")));


app.Run();
