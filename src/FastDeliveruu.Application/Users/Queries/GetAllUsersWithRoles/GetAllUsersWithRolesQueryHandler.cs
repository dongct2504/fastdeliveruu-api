using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsersWithRoles;

public class GetAllUsersWithRolesQueryHandler : IRequestHandler<GetAllUsersWithRolesQuery, PagedList<AppUserWithRolesDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICacheService _cacheService;

    public GetAllUsersWithRolesQueryHandler(UserManager<AppUser> userManager, ICacheService cacheService)
    {
        _userManager = userManager;
        _cacheService = cacheService;
    }

    public async Task<PagedList<AppUserWithRolesDto>> Handle(GetAllUsersWithRolesQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.AppUsersWithRoles}-{request.DefaultParams}";

        PagedList<AppUserWithRolesDto>? pagedListCache = await _cacheService
            .GetAsync<PagedList<AppUserWithRolesDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<AppUser> appUsersQuery = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.DefaultParams.Search))
        {
            appUsersQuery = appUsersQuery
                .Where(c => c.UserName.ToLower().Contains(request.DefaultParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.DefaultParams.Sort))
        {
            switch (request.DefaultParams.Sort)
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

        PagedList<AppUserWithRolesDto> pagedList = new PagedList<AppUserWithRolesDto>
        {
            PageNumber = request.DefaultParams.PageNumber,
            PageSize = request.DefaultParams.PageSize,
            TotalRecords = await appUsersQuery.CountAsync(cancellationToken),
            Items = await appUsersQuery
                .AsNoTracking()
                .ProjectToType<AppUserWithRolesDto>()
                .Skip((request.DefaultParams.PageNumber - 1) * request.DefaultParams.PageSize)
                .Take(request.DefaultParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
