using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace UniversityEvents.Application.Caching;

public interface IRedisCacheService
{
    Task<T> GetDataAsync<T>(string key, CancellationToken ct);
    Task SetDataAsync<T>(string key, T data, CancellationToken ct);
    Task RemoveDataAsync(string key, CancellationToken ct);
    Task RemoveByPatternAsync(string pattern, CancellationToken ct);
    Task RefreshDataAsync<T>(string key, T data, CancellationToken ct);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T> GetDataAsync<T>(string key, CancellationToken ct)
    {
        var data = await _cache.GetStringAsync(key, ct);
        return string.IsNullOrEmpty(data) ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetDataAsync<T>(string key, T data, CancellationToken ct)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), options, ct);
    }

    public async Task RemoveDataAsync(string key, CancellationToken ct)
    {
        await _cache.RemoveAsync(key, ct);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken ct)
    {
        var endpoints = _redis.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            var db = _redis.GetDatabase();

            var keys = server.Keys(pattern: $"{pattern}*").ToArray();
            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }
        }
    }

    // 🔥 Remove old cache then set new
    public async Task RefreshDataAsync<T>(string key, T data, CancellationToken ct)
    {
        await RemoveDataAsync(key, ct);
        await SetDataAsync(key, data, ct);
    }
}
