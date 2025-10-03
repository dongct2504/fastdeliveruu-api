using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler :
    IRequestHandler<GetAllUsersQuery, PagedList<AppUserDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICacheService _cacheService;

    public GetAllUsersQueryHandler(ICacheService cacheService, UserManager<AppUser> userManager)
    {
        _cacheService = cacheService;
        _userManager = userManager;
    }

    public async Task<PagedList<AppUserDto>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.AppUsers}-{request.AppUserParams}";

        PagedList<AppUserDto>? pagedListCache = await _cacheService
            .GetAsync<PagedList<AppUserDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<AppUser> appUsersQuery = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.AppUserParams.Search))
        {
            appUsersQuery = appUsersQuery
                .Where(c => c.UserName.ToLower().Contains(request.AppUserParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.AppUserParams.Sort))
        {
            switch (request.AppUserParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    appUsersQuery = appUsersQuery.OrderBy(c => c.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    appUsersQuery = appUsersQuery.OrderByDescending(c => c.UpdatedAt);
                    break;
                case SortConstants.NameAsc:
                    appUsersQuery = appUsersQuery.OrderBy(c => c.UserName);
                    break;
                case SortConstants.NameDesc:
                    appUsersQuery = appUsersQuery.OrderByDescending(c => c.UserName);
                    break;
            }
        }
        else
        {
            appUsersQuery = appUsersQuery.OrderBy(c => c.UserName);
        }

        PagedList<AppUserDto> pagedList = new PagedList<AppUserDto>
        {
            PageNumber = request.AppUserParams.PageNumber,
            PageSize = request.AppUserParams.PageSize,
            TotalRecords = await appUsersQuery.CountAsync(cancellationToken),
            Items = await appUsersQuery
                .AsNoTracking()
                .ProjectToType<AppUserDto>()
                .Skip((request.AppUserParams.PageNumber - 1) * request.AppUserParams.PageSize)
                .Take(request.AppUserParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}