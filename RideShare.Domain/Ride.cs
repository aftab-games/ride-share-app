namespace RideShare.Domain;

public class Ride
{
    public Guid Id { get; set; }
    public Guid RiderId { get; set; }
    public Guid? DriverId { get; set; }   // nullable — no driver assigned until Accepted
    public RideStatus Status { get; set; } = RideStatus.Requested;
    public required Coordinate Pickup { get; set; }
    public required Coordinate Dropoff { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}