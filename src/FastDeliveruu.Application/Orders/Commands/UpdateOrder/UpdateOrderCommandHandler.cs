﻿using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;

    public UpdateOrderCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _unitOfWork.Orders.GetAsync(request.Id);
        if (order == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.OrderNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.OrderNotFound));
        }

        order.PaymentOrderId = request.PaymentOrderId;
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
