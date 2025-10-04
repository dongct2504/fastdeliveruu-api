using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Interfaces;

namespace FastDeliveruu.Infrastructure.Services;

public class OnlineTrackerService : IOnlineTrackerService
{
    private readonly ICacheService _cacheService;

    public OnlineTrackerService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<List<string>> GetConnectionIdsForUserAsync(Guid id)
    {
        string key = $"{CacheConstants.Online}-{id}";
        List<string> connectionIds = await _cacheService.GetAsync<List<string>>(key) ?? new List<string>();
        return connectionIds;
    }

    public async Task UserConnectedAsync(Guid id, string connectionId)
    {
        string key = $"{CacheConstants.Online}-{id}";
        List<string>? connectionIds = await _cacheService.GetAsync<List<string>>(key);
        if (connectionIds == null)
        {
            connectionIds = new List<string> { connectionId };
        }
        else
        {
            connectionIds.Add(connectionId);
        }

        await _cacheService.SetAsync(key, connectionIds, CacheOptions.OnlineExpiration);
    }

    public async Task UserDisconnectedAsync(Guid id, string connectionId)
    {
        string key = $"{CacheConstants.Online}-{id}";
        List<string>? connectionIds = await _cacheService.GetAsync<List<string>>(key);

        if (connectionIds != null)
        {
            connectionIds.Remove(connectionId);

            if (!connectionIds.Any())
            {
                await _cacheService.RemoveAsync(key);
            }

            await _cacheService.SetAsync(key, connectionIds, CacheOptions.OnlineExpiration);
        }
    }
}
