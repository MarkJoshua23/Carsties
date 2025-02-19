using AuctionService.Consumers;
using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//dotnet ef migrations add "InitialCreate" -o Data/Migrations to make migrations
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{//Retrieves the connection string
    Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("DefaultConnection")}");

    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//rabbitmq
builder.Services.AddMassTransit(x =>
{
    //store failed publish to outbox then try every 10 seconds to attempt publishing
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        //use the postgres from the dbcontext
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    x.UsingRabbitMq((context, cfg) =>
    {
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


var app = builder.Build();

// Configure the HTTP request pipeline.
//Middlewares

//make sure authentication comes before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{

    Console.WriteLine(e);
}

app.Run();