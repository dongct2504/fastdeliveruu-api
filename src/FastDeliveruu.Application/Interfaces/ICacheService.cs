﻿using Microsoft.Extensions.Caching.Distributed;

namespace FastDeliveruu.Application.Interfaces;

public interface ICacheService
{
    Task<List<string>> GetKeysExcepPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task SetAsync<T>(
        string key,
        T value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default) where T : class;

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
}
