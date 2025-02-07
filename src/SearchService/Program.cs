using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

//connection to mongodb
await DB.InitAsync("SearchDb",MongoClientSettings
.FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection")));


//make model color properties will be the basis of keyword search
await DB.Index<Item>()
.Key(x=>x.Make, KeyType.Text)
.Key(x=>x.Model, KeyType.Text)
.Key(x=>x.Color, KeyType.Text)
.CreateAsync();

app.Run();
