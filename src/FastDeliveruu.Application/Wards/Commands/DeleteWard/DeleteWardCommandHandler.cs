using FastDeliveruu.Application.Common.Constants;
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

namespace FastDeliveruu.Application.Wards.Commands.DeleteWard;

public class DeleteWardCommandHandler : IRequestHandler<DeleteWardCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteWardCommandHandler> _logger;

    public DeleteWardCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteWardCommandHandler> logger,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteWardCommand request, CancellationToken cancellationToken)
    {
        Ward? ward = await _unitOfWork.Wards.GetAsync(request.Id);
        if (ward == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardNotFound));
        }

        if ((await _unitOfWork.AddressesCustomers
                .GetWithSpecAsync(new AddressesCustomerByDistrictIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardCustomerDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardCustomerDelete));
        }

        if ((await _unitOfWork.Shippers
                .GetWithSpecAsync(new ShipperByWardIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardShipperDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardShipperDelete));
        }

        if ((await _unitOfWork.Restaurants
                .GetWithSpecAsync(new RestaurantByDistrictIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardRestaurantDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardRestaurantDelete));
        }

        if ((await _unitOfWork.Orders
                .GetWithSpecAsync(new OrderByDistrictIdSpecification(request.Id), true)) != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardOrderDelete} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardOrderDelete));
        }

        _unitOfWork.Wards.Delete(ward);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.Wards, cancellationToken);

        return Result.Ok();
    }
}
