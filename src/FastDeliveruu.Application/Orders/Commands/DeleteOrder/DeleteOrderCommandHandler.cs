using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrderCommandHandler> _logger;

    public DeleteOrderCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<DeleteOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _unitOfWork.Orders.GetAsync(request.Id);
        if (order == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.OrderNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.OrderNotFound));
        }

        _unitOfWork.Orders.Delete(order);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
