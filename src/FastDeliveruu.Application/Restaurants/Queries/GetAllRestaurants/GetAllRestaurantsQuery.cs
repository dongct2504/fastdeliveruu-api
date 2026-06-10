using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQuery : IRequest<Result<PagedList<RestaurantDto>>>
{
    public GetAllRestaurantsQuery(RestaurantParams restaurantParams)
    {
        RestaurantParams = restaurantParams;
    }

    public RestaurantParams RestaurantParams { get; }
}