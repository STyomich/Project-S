using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using UsersService.Application.Interfaces;

namespace UsersService.Infrastructure.Caching.Redis;

public class RedisCacheService(IDistributedCache cache) : ICacheService
{
    private readonly IDistributedCache _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var json = await _cache.GetStringAsync(key, ct);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan ttl,
        CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            },
            ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct)
        => await _cache.RemoveAsync(key, ct);
}
