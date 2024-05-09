using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
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
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public GetAllMenuItemsQueryHandler(IMenuItemRepository menuItemRepository, IMapper mapper)
    {
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<MenuItemDetailDto>> Handle(
        GetAllMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
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
            Items = _mapper.Map<IEnumerable<MenuItemDetailDto>>(await _menuItemRepository.ListAllAsync(options)),
            TotalRecords = await _menuItemRepository.GetCountAsync()
        };

        return paginationResponse;
    }
}
