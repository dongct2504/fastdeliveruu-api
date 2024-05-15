using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.MenuItems.Queries.GetMenuItemById;

public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, Result<MenuItemDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public GetMenuItemByIdQueryHandler(
        IMenuItemRepository menuItemRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
        _cacheService = cacheService;
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

        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Genre, Restaurant",
            Where = mi => mi.MenuItemId == request.Id
        };
        MenuItem? menuItem = await _menuItemRepository.GetAsync(options, asNoTracking: true);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<MenuItemDetailDto>(new NotFoundError(message));
        }

        menuItemCache = _mapper.Map<MenuItemDetailDto>(menuItem);

        await _cacheService.SetAsync(key, menuItemCache, CacheOptions.DefaultExpiration, cancellationToken);

        return menuItemCache;
    }
}
