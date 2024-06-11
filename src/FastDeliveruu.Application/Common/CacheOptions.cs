using Microsoft.Extensions.Caching.Distributed;

namespace FastDeliveruu.Application.Common;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20) };

    public static DistributedCacheEntryOptions CartExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) };
}
