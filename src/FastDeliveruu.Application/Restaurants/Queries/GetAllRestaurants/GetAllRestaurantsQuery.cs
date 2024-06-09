using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQuery : IRequest<PagedList<RestaurantDto>>
{
    public GetAllRestaurantsQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; }

    public int PageSize { get; }
}