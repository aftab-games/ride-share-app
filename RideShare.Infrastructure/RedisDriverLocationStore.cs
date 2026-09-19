using StackExchange.Redis;

namespace RideShare.Infrastructure;

public class RedisDriverLocationStore : IDriverLocationStore
{
    private readonly IDatabase _redis;
    private const string KeyPrefix = "driver-location:";

    public RedisDriverLocationStore(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public void UpdateLocation(Guid driverId, decimal lat, decimal lng)
    {
        var value = $"{lat}|{lng}|{DateTime.UtcNow:O}";
        _redis.StringSet(KeyPrefix + driverId, value);
    }

    public void RemoveLocation(Guid driverId)
    {
        _redis.KeyDelete(KeyPrefix + driverId);
    }

    public DriverLocation? GetLocation(Guid driverId)
    {
        var value = _redis.StringGet(KeyPrefix + driverId);
        if (!value.HasValue) return null;

        var parts = ((string)value!).Split('|');
        return new DriverLocation(decimal.Parse(parts[0]), decimal.Parse(parts[1]), DateTime.Parse(parts[2]));
    }

}