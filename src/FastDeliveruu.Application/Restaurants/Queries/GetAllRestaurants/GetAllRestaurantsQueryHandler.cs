using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQueryHandler : IRequestHandler<GetAllRestaurantsQuery, PaginationResponse<RestaurantDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public GetAllRestaurantsQueryHandler(
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<PaginationResponse<RestaurantDto>> Handle(
        GetAllRestaurantsQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Restaurants}-{request.PageNumber}";

        PaginationResponse<RestaurantDto>? paginationResponseCache = await _cacheService
            .GetAsync<PaginationResponse<RestaurantDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize
        };

        PaginationResponse<RestaurantDto> paginationResponse = new PaginationResponse<RestaurantDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            Items = _mapper.Map<IEnumerable<RestaurantDto>>(
                await _restaurantRepository.ListAllAsync(options, asNoTracking: true)),
            TotalRecords = await _restaurantRepository.GetCountAsync()
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}