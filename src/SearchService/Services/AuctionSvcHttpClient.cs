using System;
using System.Net.Http.Json;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    //this calls the auction service by using a httpclient to send request
    //no actiontask since were not returning response like 404 ok etc
    public async Task<List<Item>> GetItemsForSearchDb()
    {
        //query Item but project/return string
        var lastUpdated = await DB.Find<Item, string>()
        .Sort(x => x.Descending(x => x.UpdatedAt))
        //return or project only the UpdatedAt not the whole entity
        .Project(x => x.UpdatedAt.ToString())
        //get only the first or latest
        .ExecuteFirstAsync();

        return await _httpClient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated);
    }
}
