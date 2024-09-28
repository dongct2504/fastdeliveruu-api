using FastDeliveruu.Application.Dtos.RestaurantDtos;
using MediatR;

namespace FastDeliveruu.Application.RestaurantHours.Queries.GetByRestaurant;

public class GetByRestaurantQuery : IRequest<List<RestaurantHourDto>>
{
    public GetByRestaurantQuery(Guid restaurantId)
    {
        RestaurantId = restaurantId;
    }

    public Guid RestaurantId { get; }
}
