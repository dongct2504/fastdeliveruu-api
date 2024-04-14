using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface ILocalUserServices
{
    Task<IEnumerable<LocalUser>> GetAllLocalUserAsync();
    Task<IEnumerable<LocalUser>> GetAllLocalUserAsync(int page);

    Task<Result<LocalUser>> GetLocalUser(string username);

    Task<bool> IsUserUnique(string username);

    Task<int> GetTotalLocalUsersAsync();

    Task<Result<Guid>> AddUserAsync(LocalUser localUser);
    Task<Result> UpdateUserAsync(Guid id, LocalUser localUser);
    Task<Result> DeleteUserAsync(Guid id);
}