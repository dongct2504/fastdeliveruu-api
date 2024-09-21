using FastDeliveruu.Application.Dtos.RestaurantHourDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.RestaurantHours.Queries.GetRestaurantHourById;

public class GetRestaurantHourByIdQuery : IRequest<Result<RestaurantHourDto>>
{
    public GetRestaurantHourByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
