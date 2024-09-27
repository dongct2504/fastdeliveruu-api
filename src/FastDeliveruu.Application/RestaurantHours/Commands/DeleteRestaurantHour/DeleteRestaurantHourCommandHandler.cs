using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.RestaurantHours.Commands.DeleteRestaurantHour;

public class DeleteRestaurantHourCommandHandler : IRequestHandler<DeleteRestaurantHourCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRestaurantHourCommandHandler> _logger;

    public DeleteRestaurantHourCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteRestaurantHourCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteRestaurantHourCommand request, CancellationToken cancellationToken)
    {
        RestaurantHour? restaurantHours = await _unitOfWork.RestaurantHours.GetAsync(request.Id);
        if (restaurantHours == null)
        {
            string message = "RestaurantHour not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _unitOfWork.RestaurantHours.Delete(restaurantHours);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
