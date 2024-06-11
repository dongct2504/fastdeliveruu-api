using FastDeliveruu.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FastDeliveruu.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? cacheValue = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (cacheValue == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<T>(cacheValue);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default) where T : class
    {
        string cacheValue = JsonConvert.SerializeObject(value);

        await _distributedCache.SetStringAsync(key, cacheValue, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key);
    }
}
