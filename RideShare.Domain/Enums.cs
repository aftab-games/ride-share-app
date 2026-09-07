namespace RideShare.Domain;

public enum RideStatus
{
    Requested,
    Accepted,
    Cancelled,
    InProgress,
    Completed
}

public enum DriverAvailability
{
    Offline,
    Available,
    OnTrip
}