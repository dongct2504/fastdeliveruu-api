using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.AddressesCustomers;
using FastDeliveruu.Domain.Specifications.Orders;
using FastDeliveruu.Domain.Specifications.Restaurants;
using FastDeliveruu.Domain.Specifications.Shippers;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Cities.Commands.DeleteCity;

public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteCityCommandHandler> _logger;

    public DeleteCityCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteCityCommandHandler> logger,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        City? city = await _unitOfWork.Cities.GetAsync(request.Id);
        if (city == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityNotFound));
        }

        if ((await _unitOfWork.AddressesCustomers
                .GetWithSpecAsync(new AddressesCustomerByCityIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityCustomerDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityCustomerDelete));
        }

        if ((await _unitOfWork.Shippers
                .GetWithSpecAsync(new ShipperByCityIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityShipperDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityShipperDelete));
        }

        if ((await _unitOfWork.Restaurants
                .GetWithSpecAsync(new RestaurantByCityIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityRestaurantDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityRestaurantDelete));
        }

        if ((await _unitOfWork.Orders
                .GetWithSpecAsync(new OrderByCityIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityOrderDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityOrderDelete));
        }

        _unitOfWork.Cities.Delete(city);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.Cities, cancellationToken);

        return Result.Ok();
    }
}
