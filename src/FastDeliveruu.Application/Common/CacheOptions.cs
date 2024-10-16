using Microsoft.Extensions.Caching.Distributed;

namespace FastDeliveruu.Application.Common;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20) };

    public static DistributedCacheEntryOptions CartExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) };

    public static DistributedCacheEntryOptions OnlineExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2) };

    public static DistributedCacheEntryOptions GroupChatExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2) };

    public static DistributedCacheEntryOptions OtpExpiration =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public static DistributedCacheEntryOptions TempOrderId =>
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) };
}
