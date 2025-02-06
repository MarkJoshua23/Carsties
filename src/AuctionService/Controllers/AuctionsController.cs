using System;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

//attribute to enable validations
//validates the request and payload before allowing the request to the api
[ApiController]

[Route("api/auctions")]
//inherits controllerbase for controler without view
//framework will make a new instance for every call in route
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    //make dbcontext and mapper available for every instance


    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    [HttpGet]
    //List<AuctionDto>, meaning it will return a list of AuctionDto objects.
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        //Auctions is from Dbset in AuctionDbContext
        // var auction is the reference of the real entity so u manipulate it to change entity
        var auctions = await _context.Auctions
        .Include(x => x.Item)//also load the Item Entity aside from Auction
        .OrderBy(x => x.Item.Make)
        .ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        return _mapper.Map<AuctionDto>(auction);  // Map auction to AuctionDto
    }
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        //map the dto to the parent entity, the db with no FK
        var auction = _mapper.Map<Auction>(createAuctionDto);
        // TODO: Add current user as seller
        auction.Seller = "test";

        //add the data from client 
        _context.Auctions.Add(auction);

        //if 0 changes then nothing happens , if >0 then something changed
        var results = await _context.SaveChangesAsync() > 0;

        if (!results) return BadRequest("Could not save changes");

        //return the content and location of the inserted data + the other data in AuctionDto
        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));

    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();

        //TODO: Chceck Seller == username
        //If updateAuctionDto.Make is NOT null: use the new value from updateAuctionDto
        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.MileAge = updateAuctionDto.MileAge ?? auction.Item.MileAge;// put ? in int => in? in the updateAUctiondto
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var results = await _context.SaveChangesAsync() > 0;
        if (!results) return BadRequest("Problem Saving");
        return Ok(results);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions
        .FindAsync(id);
        if (auction == null) return NotFound();

        //TODO: check seller == username
        //delete the data from db
        _context.Auctions.Remove(auction);

        var results = await _context.SaveChangesAsync() > 0;

        if (!results) return BadRequest("Could not update DB");
        return Ok();


    }

}
