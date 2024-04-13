using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface ILocalUserServices
{
    Task<IEnumerable<LocalUser>> GetAllLocalUserAsync();
    Task<IEnumerable<LocalUser>> GetAllLocalUserAsync(int page);

    Task<int> GetTotalLocalUsersAsync();

    Task<int> AddUserAsync(LocalUser localUser);
    Task UpdateUserAsync(LocalUser localUser);
    Task DeleteUserAsync(LocalUser localUser);
}