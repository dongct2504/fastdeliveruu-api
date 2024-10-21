using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.AddressesCustomers;
using FastDeliveruu.Domain.Specifications.Orders;
using FastDeliveruu.Domain.Specifications.Restaurants;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Cities.Commands.DeleteCity;

public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCityCommandHandler> _logger;

    public DeleteCityCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteCityCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        City? city = await _unitOfWork.Cities.GetAsync(request.Id);
        if (city == null)
        {
            string message = "city not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.AddressesCustomers
                .GetWithSpecAsync(new AddressesCustomerByCityIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete this city because it is used by customer(s)";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.Restaurants
                .GetWithSpecAsync(new RestaurantByCityIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete this city because it is used by restaurant(s)";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        if ((await _unitOfWork.Orders
                .GetWithSpecAsync(new OrderByCityIdSpecification(request.Id), true)) != null)
        {
            string message = "can't delete this city because it is used by order(s)";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        _unitOfWork.Cities.Delete(city);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
