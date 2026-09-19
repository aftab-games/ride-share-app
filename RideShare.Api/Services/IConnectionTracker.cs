namespace RideShare.Api.Services;

public interface IConnectionTracker
{
    void AddDriverConnection(Guid driverId, string connectionId);
    void RemoveConnection(string connectionId);
    string? GetDriverConnection(Guid driverId);
}