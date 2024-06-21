using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQuery : IRequest<PagedList<RestaurantDto>>
{
    public GetAllRestaurantsQuery(RestaurantParams restaurantParams)
    {
        RestaurantParams = restaurantParams;
    }

    public RestaurantParams RestaurantParams { get; }
}