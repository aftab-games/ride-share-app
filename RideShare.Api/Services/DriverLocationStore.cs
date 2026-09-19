using System.Collections.Concurrent;

namespace RideShare.Api.Services;

public class DriverLocationStore : IDriverLocationStore
{
    private readonly ConcurrentDictionary<Guid, DriverLocation> _locations = new();

    public void UpdateLocation(Guid driverId, decimal lat, decimal lng)
        => _locations[driverId] = new DriverLocation(lat, lng, DateTime.UtcNow);

    public void RemoveLocation(Guid driverId)
        => _locations.TryRemove(driverId, out _);

    public DriverLocation? GetLocation(Guid driverId)
        => _locations.TryGetValue(driverId, out var location) ? location : null;

    public IReadOnlyDictionary<Guid, DriverLocation> GetAllLocations()
        => _locations;
}