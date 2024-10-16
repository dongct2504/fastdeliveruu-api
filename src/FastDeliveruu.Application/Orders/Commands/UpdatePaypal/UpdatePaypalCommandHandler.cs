using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Orders;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Commands.UpdatePaypal;

public class UpdatePaypalCommandHandler : IRequestHandler<UpdatePaypalCommand, Result<PaymentResponse>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<UpdatePaypalCommandHandler> _logger;

    public UpdatePaypalCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdatePaypalCommandHandler> logger,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result<PaymentResponse>> Handle(UpdatePaypalCommand request, CancellationToken cancellationToken)
    {
        string? orderIdString = await _cacheService.GetAsync<string>("TempOrderId");
        if (orderIdString == null)
        {
            string message = "Order not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        Order? order = await _unitOfWork.Orders.GetWithSpecAsync(new OrderWithPaymentsById(Guid.Parse(orderIdString)));
        if (order == null)
        {
            string message = "Order not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        PaymentResponse paymentResponse = new PaymentResponse()
        {
            IsSuccess = true,
            OrderId = order.Id,
            OrderDescription = order.OrderDescription ?? string.Empty,
            PaymentMethod = (PaymentMethodsEnum)(order.PaymentMethod ?? 0),
            TotalAmount = order.TotalAmount,
            TransactionId = order.TransactionId ?? "0"
        };

        order.OrderStatus = (byte?)OrderStatusEnum.Success;

        await _unitOfWork.SaveChangesAsync();

        return paymentResponse;
    }
}
