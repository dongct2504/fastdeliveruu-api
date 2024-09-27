using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItems.Queries.GetMenuItemById;

public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, Result<MenuItemDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetMenuItemByIdQueryHandler> _logger;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetMenuItemByIdQueryHandler(
        ICacheService cacheService,
        FastDeliveruuDbContext dbContext,
        ILogger<GetMenuItemByIdQueryHandler> logger)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<MenuItemDetailDto>> Handle(
        GetMenuItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuItem}-{request.Id}";

        MenuItemDetailDto? menuItemCache = await _cacheService.GetAsync<MenuItemDetailDto>(key, cancellationToken);
        if (menuItemCache != null)
        {
            return menuItemCache;
        }

        MenuItemDetailDto? menuItemDetailDto = await _dbContext.MenuItems
            .Where(mi => mi.Id == request.Id)
            .AsNoTracking()
            .ProjectToType<MenuItemDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (menuItemDetailDto == null)
        {
            string message = "MenuItem not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<MenuItemDetailDto>(new NotFoundError(message));
        }

        await _cacheService.SetAsync(key, menuItemDetailDto, CacheOptions.DefaultExpiration, cancellationToken);

        return menuItemDetailDto;
    }
}
