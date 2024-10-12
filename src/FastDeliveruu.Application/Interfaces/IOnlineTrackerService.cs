namespace FastDeliveruu.Application.Interfaces;

public interface IOnlineTrackerService
{
    Task UserConnectedAsync(Guid id, string connectionId);

    Task UserDisconnectedAsync(Guid id, string connectionId);

    Task<List<string>> GetConnectionIdsForUserAsync(Guid id);
}
