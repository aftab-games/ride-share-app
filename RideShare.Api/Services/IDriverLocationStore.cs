namespace RideShare.Api.Services;

public record DriverLocation(decimal Lat, decimal Lng, DateTime UpdatedAt);

public interface IDriverLocationStore
{
    void UpdateLocation(Guid driverId, decimal lat, decimal lng);
    void RemoveLocation(Guid driverId);
    DriverLocation? GetLocation(Guid driverId);
    IReadOnlyDictionary<Guid, DriverLocation> GetAllLocations();
}