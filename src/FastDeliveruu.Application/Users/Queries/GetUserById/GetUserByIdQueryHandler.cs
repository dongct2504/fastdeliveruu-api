using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FastDeliveruu.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<LocalUserDetailDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetUserByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<Result<LocalUserDetailDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.LocalUser}-{request.Id}";

        LocalUserDetailDto? localUserDetailDtoCache = await _cacheService
            .GetAsync<LocalUserDetailDto>(key, cancellationToken);
        if (localUserDetailDtoCache != null)
        {
            return localUserDetailDtoCache;
        }

        LocalUserDetailDto? localUserDetailDto = await _dbContext.LocalUsers
            .Where(lc => lc.LocalUserId == request.Id)
            .AsNoTracking()
            .ProjectToType<LocalUserDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);
        if (localUserDetailDto == null)
        {
            string message = "User not found";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        await _cacheService.SetAsync(key, localUserDetailDto, CacheOptions.DefaultExpiration, cancellationToken);

        return localUserDetailDto;
    }
}