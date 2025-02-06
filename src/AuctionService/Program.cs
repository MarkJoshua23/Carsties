using AuctionService.Data;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
//Middlewares

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