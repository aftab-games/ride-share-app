using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideShare.Api.Contracts;
using RideShare.Domain;
using RideShare.Infrastructure;

namespace RideShare.Api.Controllers;

[ApiController]
[Route("api/rides")]
public class RidesController : ControllerBase
{
    private readonly RideShareDbContext _db;

    public RidesController(RideShareDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<RideResponse>> Create(CreateRideRequest request)
    {
        var riderExists = await _db.Riders.AnyAsync(r => r.Id == request.RiderId);
        if (!riderExists) return BadRequest($"Rider {request.RiderId} does not exist.");

        var ride = new Ride
        {
            Id = Guid.NewGuid(),
            RiderId = request.RiderId,
            Status = RideStatus.Requested,
            Pickup = new Coordinate { Lat = request.PickupLat, Lng = request.PickupLng },
            Dropoff = new Coordinate { Lat = request.DropoffLat, Lng = request.DropoffLng }
        };
        _db.Rides.Add(ride);
        await _db.SaveChangesAsync();

        var response = ToResponse(ride);
        return CreatedAtAction(nameof(GetById), new { id = ride.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RideResponse>> GetById(Guid id)
    {
        var ride = await _db.Rides.FindAsync(id);
        if (ride is null) return NotFound();

        return ToResponse(ride);
    }

    private static RideResponse ToResponse(Ride ride) => new(
        ride.Id, ride.RiderId, ride.DriverId, ride.Status.ToString(),
        ride.Pickup.Lat, ride.Pickup.Lng, ride.Dropoff.Lat, ride.Dropoff.Lng, ride.CreatedAt);
}