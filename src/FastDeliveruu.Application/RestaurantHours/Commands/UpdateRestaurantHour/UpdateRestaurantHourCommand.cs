using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.RestaurantHours.Commands.UpdateRestaurantHour;

public class UpdateRestaurantHourCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    public string? WeekenDay { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
