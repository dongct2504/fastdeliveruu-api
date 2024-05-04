using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommand : IRequest<Result>
{
    public DeleteRestaurantCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
