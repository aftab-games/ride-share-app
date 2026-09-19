using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RideShare.Api.Contracts;
using RideShare.Api.Hubs;
using RideShare.Api.Services;
using RideShare.Domain;
using RideShare.Infrastructure;

namespace RideShare.Api.Controllers;

[ApiController]
[Route("api/rides")]
public class RidesController : ControllerBase
{
    private readonly RideShareDbContext _db;
    private readonly IHubContext<RideHub> _hubContext;

    private readonly IConnectionTracker _connectionTracker;
    private readonly IDriverLocationStore _locationStore;

    public RidesController(RideShareDbContext db, IHubContext<RideHub> hubContext, IConnectionTracker connectionTracker, IDriverLocationStore locationStore)
    {
        _db = db;
        _hubContext = hubContext;
        _connectionTracker = connectionTracker;
        _locationStore = locationStore;
    }

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

        var nearestDriverId = await FindNearestAvailableDriverAsync(request.PickupLat, request.PickupLng);
        if (nearestDriverId is not null)
        {
            var connectionId = _connectionTracker.GetDriverConnection(nearestDriverId.Value);
            if (connectionId is not null)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("RideRequested", response);
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = ride.Id }, response);
    }

    private async Task<Guid?> FindNearestAvailableDriverAsync(decimal pickupLat, decimal pickupLng)
    {
        var availableDriverIds = await _db.Drivers
            .Where(d => d.Availability == DriverAvailability.Available)
            .Select(d => d.Id)
            .ToListAsync();

        Guid? nearestId = null;
        double nearestDistance = double.MaxValue;

        foreach (var driverId in availableDriverIds)
        {
            var location = _locationStore.GetLocation(driverId);
            if (location is null) continue;

            var distance = GeoUtils.DistanceKm(pickupLat, pickupLng, location.Lat, location.Lng);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestId = driverId;
            }
        }

        return nearestId;
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