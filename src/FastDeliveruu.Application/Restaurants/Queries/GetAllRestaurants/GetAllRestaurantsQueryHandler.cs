using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQueryHandler : IRequestHandler<GetAllRestaurantsQuery, PaginationResponse<RestaurantDto>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public GetAllRestaurantsQueryHandler(IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<RestaurantDto>> Handle(
        GetAllRestaurantsQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize
        };

        PaginationResponse<RestaurantDto> paginationResponse = new PaginationResponse<RestaurantDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            Items = _mapper.Map<IEnumerable<RestaurantDto>>(await _restaurantRepository.ListAllAsync(options)),
            TotalRecords = await _restaurantRepository.GetCountAsync()
        };

        return paginationResponse;
    }
}