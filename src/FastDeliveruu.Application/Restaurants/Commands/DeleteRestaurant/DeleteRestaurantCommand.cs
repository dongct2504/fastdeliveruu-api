using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommand : IRequest<Result>
{
    public DeleteRestaurantCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
