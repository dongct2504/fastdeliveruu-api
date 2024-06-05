using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Queries.SearchMenuItems;

public class SearchMenuItemsQueryHandler : IRequestHandler<SearchMenuItemsQuery, IEnumerable<MenuItemDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public SearchMenuItemsQueryHandler(
        IMenuItemRepository menuItemRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _cacheService = cacheService;
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MenuItemDto>> Handle(
        SearchMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuItems}-{request.Amount}-{request.DiscountPercent}";

        IEnumerable<MenuItemDto>? menuItemDtosCache = await _cacheService
            .GetAsync<IEnumerable<MenuItemDto>>(key, cancellationToken);
        if (menuItemDtosCache != null)
        {
            return menuItemDtosCache;
        }

        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            Where = mi => mi.Price == request.Amount && mi.DiscountPercent == request.DiscountPercent,
            PageNumber = 1,
            PageSize = 50
        };

        IEnumerable<MenuItemDto> menuItemDtos = _mapper.Map<IEnumerable<MenuItemDto>>(
            await _menuItemRepository.ListAllAsync(options, asNoTracking: true));

        await _cacheService.SetAsync(key, menuItemDtos, CacheOptions.DefaultExpiration, cancellationToken);

        return menuItemDtos;
    }
}
