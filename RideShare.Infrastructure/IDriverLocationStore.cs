namespace RideShare.Infrastructure;

public record DriverLocation(decimal Lat, decimal Lng, DateTime UpdatedAt);

public interface IDriverLocationStore
{
    void UpdateLocation(Guid driverId, decimal lat, decimal lng);
    void RemoveLocation(Guid driverId);
    DriverLocation? GetLocation(Guid driverId);
}