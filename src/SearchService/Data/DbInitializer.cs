using System;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        //connection to mongodb
        await DB.InitAsync("SearchDb", MongoClientSettings
        .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));


        //make model color properties will be the basis of keyword search
        await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text)
        .CreateAsync();

        //count the items inside the db
        var count = await DB.CountAsync<Item>();

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await httpClient.GetItemsForSearchDb();
        Console.WriteLine(items.Count + " return from service");

        if (items.Count>0) await DB.SaveAsync(items);


        // //only use this if youre going to use a json
        // if (count == 0)
        // {
        //     Console.WriteLine("No data found :( - seeding data...");

        //     //get the json from auctions.json
        //     var itemData = await File.ReadAllTextAsync("Data/auctions.json");

        //     //used in json to list
        //     var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        //     //json to list
        //     var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

        //     //save to db
        //     await DB.SaveAsync(items);
        // }
    }
}
