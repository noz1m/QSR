using StackExchange.Redis;

namespace QSR.Tests;

public class RedisTests
{
    [RedisFact]
    public async Task CacheValueCanBeWrittenReadAndRemoved()
    {
        using var connection = await ConnectionMultiplexer.ConnectAsync(
            Environment.GetEnvironmentVariable("QSR_TEST_REDIS")!);
        var db = connection.GetDatabase();
        var key = $"qsr-test:{Guid.NewGuid():N}";
        try
        {
            Assert.True(await db.StringSetAsync(key, "snapshot", TimeSpan.FromMinutes(1)));
            Assert.Equal("snapshot", (string?)await db.StringGetAsync(key));
            Assert.True(await db.KeyDeleteAsync(key));
            Assert.True((await db.StringGetAsync(key)).IsNull);
        }
        finally
        {
            await db.KeyDeleteAsync(key);
        }
    }
}

public sealed class RedisFactAttribute : FactAttribute
{
    public RedisFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("QSR_TEST_REDIS")))
            Skip = "Set QSR_TEST_REDIS to a test Redis instance.";
    }
}
