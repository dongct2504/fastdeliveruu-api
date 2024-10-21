using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.AddressesCustomers;
using FastDeliveruu.Domain.Specifications.Orders;
using FastDeliveruu.Domain.Specifications.Restaurants;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Wards.Commands.DeleteWard;

public class DeleteWardCommandHandler : IRequestHandler<DeleteWardCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteWardCommandHandler> _logger;

    public DeleteWardCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteWardCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteWardCommand request, CancellationToken cancellationToken)
    {
        Ward? ward = await _unitOfWork.Wards.GetAsync(request.Id);
        if (ward == null)
        {
            string message = "ward not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.AddressesCustomers
                .GetWithSpecAsync(new AddressesCustomerByDistrictIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete ward because it is used by customer(s).";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.Restaurants
                .GetWithSpecAsync(new RestaurantByDistrictIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete ward because it is used by restaurant(s).";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.Orders
                .GetWithSpecAsync(new OrderByDistrictIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete ward because it is used by order(s).";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        _unitOfWork.Wards.Delete(ward);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
