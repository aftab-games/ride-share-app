namespace RideShare.Api.Contracts;

public record CreateRideRequest(
    Guid RiderId,
    decimal PickupLat, decimal PickupLng,
    decimal DropoffLat, decimal DropoffLng);

public record RideResponse(
    Guid Id, Guid RiderId, Guid? DriverId, string Status,
    decimal PickupLat, decimal PickupLng,
    decimal DropoffLat, decimal DropoffLng,
    DateTime CreatedAt);