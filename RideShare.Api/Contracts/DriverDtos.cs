namespace RideShare.Api.Contracts;

public record CreateDriverRequest(string UserId, string VehicleInfo);

public record DriverResponse(Guid Id, string UserId, string Availability, string VehicleInfo);