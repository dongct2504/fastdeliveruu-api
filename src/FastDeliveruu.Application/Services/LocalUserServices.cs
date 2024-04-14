using Dapper;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

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

    public async Task<Result<LocalUser>> GetLocalUser(string username)
    {
        QueryOptions<LocalUser> options = new QueryOptions<LocalUser>
        {
            Where = u => u.UserName == username
        };

        LocalUser? localUser = await _localUserRepository.GetAsync(options);
        if (localUser == null)
        {
            return Result.Fail<LocalUser>(new NotFoundError("the requested local user is not found."));
        }

        return localUser;
    }

    public async Task<bool> IsUserUnique(string username)
    {
        QueryOptions<LocalUser> options = new QueryOptions<LocalUser>
        {
            Where = u => u.UserName == username
        };

        return await _localUserRepository.GetAsync(options) != null;
    }

    public async Task<int> GetTotalLocalUsersAsync()
    {
        return await _localUserRepository.GetCountAsync();
    }

    public async Task<Result<Guid>> AddUserAsync(LocalUser localUser)
    {
        LocalUser? isLocalUserExist = await _localUserRepository.GetAsync(localUser.LocalUserId);
        if (isLocalUserExist != null)
        {
            return Result.Fail<Guid>(
                new DuplicateError($"the requested user is already exist."));
        }

        LocalUser createdUser = await _localUserRepository.AddAsync(localUser);

        return createdUser.LocalUserId;
    }

    public async Task<Result> UpdateUserAsync(Guid id, LocalUser localUser)
    {
        LocalUser? isLocalUserExist = await _localUserRepository.GetAsync(id);
        if (isLocalUserExist == null)
        {
            return Result.Fail(
                new NotFoundError($"the requested user is not found."));
        }

        await _localUserRepository.UpdateLocalUser(localUser);

        return Result.Ok();
    }

    public async Task<Result> DeleteUserAsync(Guid id)
    {
        LocalUser? isLocalUserExist = await _localUserRepository.GetAsync(id);
        if (isLocalUserExist == null)
        {
            return Result.Fail(
                new NotFoundError($"the requested user is not found."));
        }

        await _localUserRepository.DeleteAsync(isLocalUserExist);

        return Result.Ok();
    }
}