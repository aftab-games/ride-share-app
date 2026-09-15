using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideShare.Api.Contracts;
using RideShare.Domain;
using RideShare.Infrastructure;

namespace RideShare.Api.Controllers;

[ApiController]
[Route("api/riders")]
public class RidersController : ControllerBase
{
    private readonly RideShareDbContext _db;

    public RidersController(RideShareDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<RiderResponse>> Create(CreateRiderRequest request)
    {
        var rider = new Rider { Id = Guid.NewGuid(), UserId = request.UserId };
        _db.Riders.Add(rider);
        await _db.SaveChangesAsync();

        var response = new RiderResponse(rider.Id, rider.UserId);
        return CreatedAtAction(nameof(GetById), new { id = rider.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RiderResponse>> GetById(Guid id)
    {
        var rider = await _db.Riders.FindAsync(id);
        if (rider is null) return NotFound();

        return new RiderResponse(rider.Id, rider.UserId);
    }
}