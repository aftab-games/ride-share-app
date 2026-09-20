using RideShare.Domain;
using Xunit;

namespace RideShare.Tests;

public class RideStateMachineTests
{
    private static Ride CreateRide(RideStatus status) => new()
    {
        Id = Guid.NewGuid(),
        RiderId = Guid.NewGuid(),
        Status = status,
        Pickup = new Coordinate { Lat = 0, Lng = 0 },
        Dropoff = new Coordinate { Lat = 0, Lng = 0 }
    };

    [Theory]
    [InlineData(RideStatus.Requested, RideStatus.Accepted)]
    [InlineData(RideStatus.Requested, RideStatus.Cancelled)]
    [InlineData(RideStatus.Accepted, RideStatus.Cancelled)]
    [InlineData(RideStatus.Accepted, RideStatus.InProgress)]
    [InlineData(RideStatus.InProgress, RideStatus.Completed)]
    public void CanTransitionTo_AllowsValidTransitions(RideStatus from, RideStatus to)
    {
        var ride = CreateRide(from);

        Assert.True(ride.CanTransitionTo(to));
    }

    [Theory]
    [InlineData(RideStatus.Requested, RideStatus.InProgress)]
    [InlineData(RideStatus.Requested, RideStatus.Completed)]
    [InlineData(RideStatus.Accepted, RideStatus.Requested)]
    [InlineData(RideStatus.Accepted, RideStatus.Completed)]
    [InlineData(RideStatus.InProgress, RideStatus.Requested)]
    [InlineData(RideStatus.InProgress, RideStatus.Cancelled)]
    [InlineData(RideStatus.Completed, RideStatus.Requested)]
    [InlineData(RideStatus.Completed, RideStatus.Accepted)]
    public void CanTransitionTo_RejectsInvalidTransitions(RideStatus from, RideStatus to)
    {
        var ride = CreateRide(from);

        Assert.False(ride.CanTransitionTo(to));
    }
}