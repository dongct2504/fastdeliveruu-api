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
        string key = $"{CacheConstants.AppUsers}-{request.PageNumber}-{request.PageSize}";

        PagedList<AppUserDto>? pagedListCache = await _cacheService
            .GetAsync<PagedList<AppUserDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        PagedList<AppUserDto> pagedList = new PagedList<AppUserDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = await _userManager.Users.CountAsync(cancellationToken),
            Items = await _userManager.Users
                .AsNoTracking()
                .ProjectToType<AppUserDto>()
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}