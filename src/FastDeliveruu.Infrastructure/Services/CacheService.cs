using FastDeliveruu.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace FastDeliveruu.Infrastructure.Services;

public class CacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<List<string>> GetKeysExcepPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        List<string> keys = CacheKeys.Keys
            .Where(k => k.StartsWith(prefixKey))
            .Select(k => k.Substring(prefixKey.Length + 1)) // exclude the "-"
            .ToList();

        return await Task.FromResult(keys);
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

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        IEnumerable<Task> tasks = CacheKeys.Keys
            .Where(k => k.StartsWith(prefixKey))
            .Select(k => RemoveAsync(k, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
