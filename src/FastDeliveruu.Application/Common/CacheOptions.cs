using Microsoft.Extensions.Caching.Distributed;

namespace FastDeliveruu.Application.Common;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20) };
}
