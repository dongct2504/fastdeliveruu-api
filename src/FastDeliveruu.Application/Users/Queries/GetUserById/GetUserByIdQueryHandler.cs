using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<AppUserDetailDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;
    private readonly ICacheService _cacheService;

    public GetUserByIdQueryHandler(
        UserManager<AppUser> userManager,
        ICacheService cacheService,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userManager = userManager;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<AppUserDetailDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.AppUser}-{request.Id}";

        AppUserDetailDto? appUserDetailDtoCache = await _cacheService
            .GetAsync<AppUserDetailDto>(key);
        if (appUserDetailDtoCache != null)
        {
            return appUserDetailDtoCache;
        }

        AppUserDetailDto? userDetailDto = await _userManager.Users
            .Where(u => u.Id == request.Id)
            .AsNoTracking()
            .ProjectToType<AppUserDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (userDetailDto == null)
        {
            string message = "User not found";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        await _cacheService.SetAsync(key, userDetailDto, CacheOptions.DefaultExpiration, cancellationToken);

        return userDetailDto;
    }
}