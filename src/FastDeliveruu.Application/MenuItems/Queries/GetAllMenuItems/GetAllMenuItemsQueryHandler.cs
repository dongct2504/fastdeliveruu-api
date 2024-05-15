using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Queries.GetAllMenuItems;

public class GetAllMenuItemsQueryHandler : IRequestHandler<GetAllMenuItemsQuery,
    PaginationResponse<MenuItemDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public GetAllMenuItemsQueryHandler(
        IMenuItemRepository menuItemRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<PaginationResponse<MenuItemDetailDto>> Handle(
        GetAllMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuItems}-{request.GenreId}-{request.RestaurantId}-{request.PageNumber}";

        PaginationResponse<MenuItemDetailDto>? paginationResponseCache = await _cacheService
            .GetAsync<PaginationResponse<MenuItemDetailDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Genre, Restaurant",
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize
        };

        if (request.GenreId != null)
        {
            options.Where = mi => mi.GenreId == request.GenreId;
        }

        if (request.RestaurantId != null)
        {
            options.Where = mi => mi.RestaurantId == request.RestaurantId;
        }

        PaginationResponse<MenuItemDetailDto> paginationResponse = new PaginationResponse<MenuItemDetailDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            // must be above the TotalRecords bc it has multiple Where clauses
            Items = _mapper.Map<IEnumerable<MenuItemDetailDto>>(
                await _menuItemRepository.ListAllAsync(options, asNoTracking: true)),
            TotalRecords = await _menuItemRepository.GetCountAsync()
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}
