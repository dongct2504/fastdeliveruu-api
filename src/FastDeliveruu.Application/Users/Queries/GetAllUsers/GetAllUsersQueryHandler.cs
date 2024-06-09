using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler :
    IRequestHandler<GetAllUsersQuery, PagedList<LocalUserDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetAllUsersQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<PagedList<LocalUserDto>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.LocalUsers}-{request.PageNumber}-{request.PageSize}";

        PagedList<LocalUserDto>? pagingResponseCache = await _cacheService
            .GetAsync<PagedList<LocalUserDto>>(key, cancellationToken);
        if (pagingResponseCache != null)
        {
            return pagingResponseCache;
        }

        PagedList<LocalUserDto> paginationResponse = new PagedList<LocalUserDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = await _dbContext.LocalUsers.CountAsync(cancellationToken),
            Items = await _dbContext.LocalUsers
                .AsNoTracking()
                .ProjectToType<LocalUserDto>()
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}