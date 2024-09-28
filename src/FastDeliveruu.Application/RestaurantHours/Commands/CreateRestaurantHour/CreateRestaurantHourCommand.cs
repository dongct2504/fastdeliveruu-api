using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.RestaurantHours.Commands.CreateRestaurantHour;

public class CreateRestaurantHourCommand : IRequest<Result<RestaurantHourDto>>
{
    public Guid RestaurantId { get; set; }

    public string? WeekenDay { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
