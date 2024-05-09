using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdQuery : IRequest<Result<RestaurantDetailDto>>
{
    public GetRestaurantByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}