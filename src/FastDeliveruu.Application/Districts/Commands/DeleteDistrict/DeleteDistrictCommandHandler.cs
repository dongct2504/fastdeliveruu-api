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

namespace FastDeliveruu.Application.Districts.Commands.DeleteDistrict;

public class DeleteDistrictCommandHandler : IRequestHandler<DeleteDistrictCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteDistrictCommandHandler> _logger;

    public DeleteDistrictCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteDistrictCommandHandler> logger,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteDistrictCommand request, CancellationToken cancellationToken)
    {
        District? district = await _unitOfWork.Districts.GetAsync(request.Id);
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }

        if ((await _unitOfWork.AddressesCustomers
                .GetWithSpecAsync(new AddressesCustomerByDistrictIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictCustomerDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictCustomerDelete));
        }

        if ((await _unitOfWork.Shippers
                .GetWithSpecAsync(new ShipperByDistrictIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictShipperDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictShipperDelete));
        }

        if ((await _unitOfWork.Restaurants
                .GetWithSpecAsync(new RestaurantByDistrictIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictRestaurantDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictRestaurantDelete));
        }

        if ((await _unitOfWork.Orders
                .GetWithSpecAsync(new OrderByDistrictIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictOrderDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictOrderDelete));
        }

        _unitOfWork.Districts.Delete(district);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.Districts, cancellationToken);

        return Result.Ok();
    }
}
