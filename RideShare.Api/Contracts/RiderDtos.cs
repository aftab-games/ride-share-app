namespace RideShare.Api.Contracts;

public record CreateRiderRequest(string UserId);

public record RiderResponse(Guid Id, string UserId);