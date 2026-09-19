using System.Collections.Concurrent;

namespace RideShare.Api.Services;

public class ConnectionTracker : IConnectionTracker
{
    private readonly ConcurrentDictionary<Guid, string> _driverToConnection = new();
    private readonly ConcurrentDictionary<string, Guid> _connectionToDriver = new();

    public void AddDriverConnection(Guid driverId, string connectionId)
    {
        _driverToConnection[driverId] = connectionId;
        _connectionToDriver[connectionId] = driverId;
    }

    public void RemoveConnection(string connectionId)
    {
        if (_connectionToDriver.TryRemove(connectionId, out var driverId))
        {
            _driverToConnection.TryRemove(driverId, out _);
        }
    }

    public string? GetDriverConnection(Guid driverId)
        => _driverToConnection.TryGetValue(driverId, out var connectionId) ? connectionId : null;
}