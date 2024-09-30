using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.RestaurantHours.Commands.UpdateRestaurantHour;

public class UpdateRestaurantHourCommandHandler : IRequestHandler<UpdateRestaurantHourCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRestaurantHourCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UpdateRestaurantHourCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateRestaurantHourCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateRestaurantHourCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _unitOfWork.Restaurants.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        RestaurantHour? restaurantHour = await _unitOfWork.RestaurantHours.GetAsync(request.Id);
        if (restaurantHour == null)
        {
            string message = "RestaurantHour not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, restaurantHour);
        restaurantHour.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
