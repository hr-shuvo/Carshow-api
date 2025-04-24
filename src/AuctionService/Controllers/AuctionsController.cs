using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await _context.Auctions.Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();
        
        var dataToReturn = _mapper.Map<List<AuctionDto>>(auctions);
        
        return dataToReturn;
    }

    [HttpGet("{id}", Name = "GetAuction")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction == null) return NotFound();
        
        var dataToReturn = _mapper.Map<AuctionDto>(auction);
        
        return dataToReturn;
    }

    [HttpPost]
    public async Task<ActionResult<List<AuctionDto>>> CreateAuction(AuctionCreateDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        
        // TODO: add current user as seller
        auction.Seller = "Test User";
        
        _context.Auctions.Add(auction);
        var result = await _context.SaveChangesAsync() > 0;
        
        if(!result) return BadRequest("Could not save changes to the DB");
        
        return CreatedAtAction(nameof(GetAuctionById), 
            new { id = auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, AuctionUpdateDto auctionDto)
    {
        var auction = await _context.Auctions
            .Include(auction => auction.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction == null) return NotFound();
        
        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;
        
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();
        
        return BadRequest("Could not update the auction");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        
        if(auction == null) return NotFound();
        
        //TODO - check seller == username
        
        _context.Auctions.Remove(auction);
        
        var result = await _context.SaveChangesAsync() > 0;
        
        if(result) return Ok();
        
        return BadRequest("Could not delete the auction");
    }
    
}