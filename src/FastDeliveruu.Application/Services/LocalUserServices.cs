using Dapper;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Application.Services;

public class localUserServices : ILocalUserServices
{
    private readonly ILocalUserRepository _localUserRepository;

    public localUserServices(ILocalUserRepository localUserRepository)
    {
        _localUserRepository = localUserRepository;
    }

    public async Task<IEnumerable<LocalUser>> GetAllLocalUserAsync()
    {
        return await _localUserRepository.ListAllAsync();
    }

    public async Task<IEnumerable<LocalUser>> GetAllLocalUserAsync(int page)
    {
        QueryOptions<LocalUser> options = new QueryOptions<LocalUser>
        {
            PageNumber = page,
            PageSize = PagingConstants.UserPageSize
        };

        return await _localUserRepository.ListAllAsync(options);
    }

    public async Task<int> GetTotalLocalUsersAsync()
    {
        return await _localUserRepository.GetCountAsync();
    }

    public async Task<int> AddUserAsync(LocalUser localUser)
    {
        LocalUser createdUser = await _localUserRepository.AddAsync(localUser);
        return createdUser.LocalUserId;
    }

    public async Task UpdateUserAsync(LocalUser localUser)
    {
        await _localUserRepository.UpdateLocalUser(localUser);
    }

    public async Task DeleteUserAsync(LocalUser localUser)
    {
        await _localUserRepository.DeleteAsync(localUser);
    }
}