using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.RestaurantHours.Commands.DeleteRestaurantHour;

public class DeleteRestaurantHourCommand : IRequest<Result>
{
    public DeleteRestaurantHourCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
