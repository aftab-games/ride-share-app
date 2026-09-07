namespace RideShare.Domain;

public class Driver
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public DriverAvailability Availability { get; set; } = DriverAvailability.Offline;
    public required string VehicleInfo { get; set; }
}