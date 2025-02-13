using System;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

//attribute to enable validations
//validates the request and payload before allowing the request to the api
[ApiController]

[Route("api/auctions")]
//inherits controllerbase for controller without view
//framework will make a new instance for every call in route
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    //make dbcontext and mapper available for every instance
    //also add mass transit IPublishEndpoint
    public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    // List<AuctionDto>, meaning it will return a list of AuctionDto objects.
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)//to know which data to send
    {
        //asqueryable so we can still manipulate and query after its ordered
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            //updatedat compared to date| only return dates newer than the date of the other service
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        //project automatically joins db if it has FK, convert entity to dto
        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();

        // // Auctions is from Dbset in AuctionDbContext
        // // var auction is the reference of the real entity so you manipulate it to change entity
        // var auctions = await _context.Auctions
        //     .Include(x => x.Item) // SQL: JOIN Item ON Auction.ItemId = Item.Id
        //     .OrderBy(x => x.Item.Make) // SQL: ORDER BY Item.Make
        //     .ToListAsync(); // SQL: SELECT * FROM Auctions JOIN Item ORDER BY Item.Make
        // return _mapper.Map<List<AuctionDto>>(auctions); // Return mapped DTO instead of entity
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item) // SQL: JOIN Item ON Auction.ItemId = Item.Id
            .FirstOrDefaultAsync(x => x.Id == id); // SQL: SELECT * FROM Auctions JOIN Item WHERE Auction.Id = id

        if (auction == null)
        {
            return NotFound(); // SQL: RETURN 404 if not found
        }

        return _mapper.Map<AuctionDto>(auction); // Map auction entity to AuctionDto
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        // Map the DTO to the parent entity, the DB with no FK
        var auction = _mapper.Map<Auction>(createAuctionDto); // SQL: INSERT INTO Auctions (fields) VALUES (...)
        // TODO: Add current user as seller
        auction.Seller = "test"; // Example hardcoded seller

        // Add the data from client
        _context.Auctions.Add(auction); // SQL: INSERT INTO Auctions (fields)


        //get the new auction item
        var newAuction = _mapper.Map<AuctionDto>(auction);
        //publish to outbox first, wait for the db to save
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));



        //if the publish fails then the whole transaction fails
        //if the save db fails, the outbox
        // If 0 changes, then nothing happens; if > 0, then something changed
        var results = await _context.SaveChangesAsync() > 0; // SQL: COMMIT; if no rows affected, rollback

        if (!results) return BadRequest("Could not save changes"); // Return 400 if save fails

        // Return the content and location of the inserted data + the other data in AuctionDto
        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
        // SQL: RETURN 201 Created, location header with new auction Id
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id); // SQL: SELECT * FROM Auctions JOIN Item WHERE Auction.Id = id

        if (auction == null) return NotFound(); // Return 404 if auction is not found

        // TODO: Check Seller == username
        // If updateAuctionDto.Make is NOT null: use the new value from updateAuctionDto
        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make; // SQL: UPDATE Item SET Make = @Make WHERE Id = @Id
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model; // Same for Model, Color, etc.
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.MileAge = updateAuctionDto.MileAge ?? auction.Item.MileAge;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var results = await _context.SaveChangesAsync() > 0; // SQL: COMMIT if rows affected

        if (!results) return BadRequest("Problem Saving"); // Return 400 if update fails
        return Ok(results); // Return 200 with the result
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions
            .FindAsync(id); // SQL: SELECT * FROM Auctions WHERE Id = id

        if (auction == null) return NotFound(); // Return 404 if auction is not found

        // TODO: Check Seller == username
        // Delete the data from DB
        _context.Auctions.Remove(auction); // SQL: DELETE FROM Auctions WHERE Id = @Id

        var results = await _context.SaveChangesAsync() > 0; // SQL: COMMIT if rows affected

        if (!results) return BadRequest("Could not update DB"); // Return 400 if delete fails
        return Ok(); // Return 200
    }
}
