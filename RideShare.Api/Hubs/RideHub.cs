using Microsoft.AspNetCore.SignalR;
using RideShare.Api.Services;
using RideShare.Domain;
using RideShare.Infrastructure;

namespace RideShare.Api.Hubs;

public class RideHub : Hub
{
    private readonly IConnectionTracker _connectionTracker;
    private readonly RideShareDbContext _db;
    private readonly IDriverLocationStore _locationStore;

    public RideHub(IConnectionTracker connectionTracker, RideShareDbContext db, IDriverLocationStore locationStore)
    {
        _connectionTracker = connectionTracker;
        _db = db;
        _locationStore = locationStore;
    }

    public void UpdateLocation(Guid driverId, decimal lat, decimal lng)
    {
        _locationStore.UpdateLocation(driverId, lat, lng);
        Console.WriteLine($"Driver {driverId} location updated: ({lat}, {lng})");
    }

    public void RegisterAsDriver(Guid driverId)
    {
        _connectionTracker.AddDriverConnection(driverId, Context.ConnectionId);
        Console.WriteLine($"Driver {driverId} registered on connection {Context.ConnectionId}");
    }

    public async Task SetAvailable(Guid driverId)
    {
        var driver = await _db.Drivers.FindAsync(driverId);
        if (driver is not null)
        {
            driver.Availability = DriverAvailability.Available;
            await _db.SaveChangesAsync();
        }
    }

    public void RegisterAsRider(Guid riderId)
    {
        _connectionTracker.AddRiderConnection(riderId, Context.ConnectionId);
        Console.WriteLine($"Rider {riderId} registered on connection {Context.ConnectionId}");
    }

    public async Task AcceptRide(Guid rideId, Guid driverId)
    {
        var ride = await _db.Rides.FindAsync(rideId);
        if (ride is null || ride.Status != RideStatus.Requested)
        {
            await Clients.Caller.SendAsync("RideAcceptFailed", rideId);
            return;
        }

        ride.Status = RideStatus.Accepted;
        ride.DriverId = driverId;
        await _db.SaveChangesAsync();

        var riderConnectionId = _connectionTracker.GetRiderConnection(ride.RiderId);
        if (riderConnectionId is not null)
        {
            await Clients.Client(riderConnectionId).SendAsync("RideAccepted", rideId, driverId);
        }

        Console.WriteLine($"Ride {rideId} accepted by driver {driverId}");
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionTracker.RemoveConnection(Context.ConnectionId);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}