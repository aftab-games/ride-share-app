using System.Collections.Concurrent;

namespace RideShare.Api.Services;

public class ConnectionTracker : IConnectionTracker
{
    private readonly ConcurrentDictionary<Guid, string> _driverToConnection = new();
    private readonly ConcurrentDictionary<Guid, string> _riderToConnection = new();
    private readonly ConcurrentDictionary<string, Guid> _connectionToUser = new();

    public void AddDriverConnection(Guid driverId, string connectionId)
    {
        _driverToConnection[driverId] = connectionId;
        _connectionToUser[connectionId] = driverId;
    }

    public void AddRiderConnection(Guid riderId, string connectionId)
    {
        _riderToConnection[riderId] = connectionId;
        _connectionToUser[connectionId] = riderId;
    }

    public void RemoveConnection(string connectionId)
    {
        if (_connectionToUser.TryRemove(connectionId, out var userId))
        {
            _driverToConnection.TryRemove(userId, out _);
            _riderToConnection.TryRemove(userId, out _);
        }
    }

    public string? GetDriverConnection(Guid driverId)
        => _driverToConnection.TryGetValue(driverId, out var connectionId) ? connectionId : null;

    public string? GetRiderConnection(Guid riderId)
        => _riderToConnection.TryGetValue(riderId, out var connectionId) ? connectionId : null;
}