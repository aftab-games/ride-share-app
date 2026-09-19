using Microsoft.AspNetCore.SignalR;
using RideShare.Api.Services;

namespace RideShare.Api.Hubs;

public class RideHub : Hub
{
    private readonly IConnectionTracker _connectionTracker;

    public RideHub(IConnectionTracker connectionTracker)
    {
        _connectionTracker = connectionTracker;
    }

    public void RegisterAsDriver(Guid driverId)
    {
        _connectionTracker.AddDriverConnection(driverId, Context.ConnectionId);
        Console.WriteLine($"Driver {driverId} registered on connection {Context.ConnectionId}");
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