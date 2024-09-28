using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.AddressesCustomers;
using FastDeliveruu.Domain.Specifications.Orders;
using FastDeliveruu.Domain.Specifications.Restaurants;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Districts.Commands.DeleteDistrict;

public class DeleteDistrictCommandHandler : IRequestHandler<DeleteDistrictCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteDistrictCommandHandler> _logger;

    public DeleteDistrictCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteDistrictCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteDistrictCommand request, CancellationToken cancellationToken)
    {
        District? district = await _unitOfWork.Districts.GetAsync(request.Id);
        if (district == null)
        {
            string message = "district not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        if ((await _unitOfWork.AddressesCustomers
                .GetWithSpecAsync(new AddressesCustomerByDistrictIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete district because it is used by customer(s).";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.Restaurants
                .GetWithSpecAsync(new RestaurantByDistrictIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete district because it is used by restaurant(s).";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.Orders
                .GetWithSpecAsync(new OrderByDistrictIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete district because it is used by order(s).";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        _unitOfWork.Districts.Delete(district);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
