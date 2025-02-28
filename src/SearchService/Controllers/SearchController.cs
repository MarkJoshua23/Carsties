using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    //get search items
    [HttpGet]
    //[FromQuery] to make it look in the url
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        //for text search
        // var query = DB.Find<Item>();

        //for pagination
        var query = DB.PagedSearch<Item, Item>();

        if (!string.IsNullOrEmpty(searchParams.searchTerm))
        {
            query.Match(Search.Full, searchParams.searchTerm).SortByTextScore();
        }

        //order
        query = searchParams.OrderBy switch
        {
            //sort by make then sort again by model
            "make" => query.Sort(x => x.Ascending(a => a.Make)).Sort(x => x.Ascending(a => a.Model)),
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
        };

        //filter
        query = searchParams.FilterBy switch
        {
            //if auction ends before this day
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            //items that are about tp be finished in 6 hours
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        //if seller has input, then display only the items of that seller
        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller == searchParams.Seller);
        }
        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Winner == searchParams.Winner);
        }

        //pagination parameters(only available in pagination)
        query.PageNumber(searchParams.pageNumber);
        query.PageSize(searchParams.pageSize);

        var result = await query.ExecuteAsync();
        return Ok(new
        {
            //results
            results = result.Results,
            //to know how many pages the frontend need to render in pagination ui
            pageCount = result.PageCount,
            //how many results total
            totalCount = result.TotalCount
        });
    }
}
