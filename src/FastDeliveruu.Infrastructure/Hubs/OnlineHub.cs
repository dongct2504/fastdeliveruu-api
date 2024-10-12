using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace FastDeliveruu.Infrastructure.Hubs;

public class OnlineHub : Hub
{
    private readonly IOnlineTrackerService _onlineTrackerService;

    public OnlineHub(IOnlineTrackerService onlineTrackerService)
    {
        _onlineTrackerService = onlineTrackerService;
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User != null)
        {
            await _onlineTrackerService.UserConnectedAsync(Context.User.GetCurrentUserId(), Context.ConnectionId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User != null)
        {
            await _onlineTrackerService.UserDisconnectedAsync(Context.User.GetCurrentUserId(),
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
