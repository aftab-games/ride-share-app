using RideShare.Domain;
using Xunit;

namespace RideShare.Tests;

public class GeoUtilsTests
{
    [Fact]
    public void DistanceKm_SamePoint_ReturnsZero()
    {
        var distance = GeoUtils.DistanceKm(23.8103m, 90.4125m, 23.8103m, 90.4125m);

        Assert.Equal(0, distance, precision: 3);
    }

    [Fact]
    public void DistanceKm_KnownDhakaPoints_ReturnsExpectedApproximateDistance()
    {
        // Motijheel to Gulshan, Dhaka — roughly 7-8 km apart in reality
        var distance = GeoUtils.DistanceKm(23.7330m, 90.4172m, 23.7925m, 90.4078m);

        Assert.InRange(distance, 6.0, 9.0);
    }
}