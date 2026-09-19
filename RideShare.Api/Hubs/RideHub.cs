using Microsoft.AspNetCore.SignalR;
using RideShare.Api.Services;
using RideShare.Domain;
using RideShare.Infrastructure;

namespace RideShare.Api.Hubs;

public class RideHub : Hub
{
    private readonly IConnectionTracker _connectionTracker;
    private readonly RideShareDbContext _db;

    public RideHub(IConnectionTracker connectionTracker, RideShareDbContext db)
    {
        _connectionTracker = connectionTracker;
        _db = db;
    }

    public void RegisterAsDriver(Guid driverId)
    {
        _connectionTracker.AddDriverConnection(driverId, Context.ConnectionId);
        Console.WriteLine($"Driver {driverId} registered on connection {Context.ConnectionId}");
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