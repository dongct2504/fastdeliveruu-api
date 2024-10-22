using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Restaurants;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.RestaurantHours.Commands.CreateRestaurantHour;

public class CreateRestaurantHourCommandHandler : IRequestHandler<CreateRestaurantHourCommand, Result<RestaurantHourDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRestaurantHourCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateRestaurantHourCommandHandler(
        ILogger<CreateRestaurantHourCommandHandler> logger,
        IFastDeliveruuUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<Result<RestaurantHourDto>> Handle(CreateRestaurantHourCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _unitOfWork.Restaurants.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.RestaurantNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.RestaurantNotFound));
        }

        var spec = new RestaurantHoursExistInRestaurantSpecification(request.RestaurantId, request.WeekenDay, request.StartTime, request.EndTime);

        RestaurantHour? restaurantHour = await _unitOfWork.RestaurantHours.GetWithSpecAsync(spec, true);
        if (restaurantHour != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.RestaurantHourDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.RestaurantHourDuplicate));
        }

        restaurantHour = _mapper.Map<RestaurantHour>(request);
        restaurantHour.Id = Guid.NewGuid();
        restaurantHour.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.RestaurantHours.Add(restaurantHour);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RestaurantHourDto>(restaurantHour);
    }
}
