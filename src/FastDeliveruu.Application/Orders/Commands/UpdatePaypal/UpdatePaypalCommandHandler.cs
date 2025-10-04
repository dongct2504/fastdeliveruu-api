using FastDeliveruu.Common.Constants;
using FastDeliveruu.Common.Enums;
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
    private readonly IMailNotificationService _orderNotificationService;
    private readonly ILogger<UpdatePaypalCommandHandler> _logger;

    public UpdatePaypalCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdatePaypalCommandHandler> logger,
        IMailNotificationService orderNotificationService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _orderNotificationService = orderNotificationService;
    }

    public async Task<Result<PaymentResponse>> Handle(UpdatePaypalCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _unitOfWork.Orders.GetWithSpecAsync(new OrderWithPaymentsByPaymentOrderId(request.CaptureOrderResponse.id));
        if (order == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.OrderNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.OrderNotFound));
        }

        Payment? payment = order.Payments.FirstOrDefault();
        if (payment == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.PaymentNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.PaymentNotFound));
        }

        string? transactionId = request.CaptureOrderResponse.purchase_units
            .FirstOrDefault()?
            .payments?
            .captures?
            .FirstOrDefault()?
            .id;

        order.OrderStatus = (byte?)OrderStatusEnum.Success;
        order.TransactionId = transactionId;

        payment.PaymentStatus = (byte)PaymentStatusEnum.Approved;
        payment.TransactionId = transactionId;

        PaymentResponse paymentResponse = new PaymentResponse()
        {
            IsSuccess = true,
            OrderId = order.Id,
            OrderDescription = order.OrderDescription ?? string.Empty,
            PaymentMethod = (PaymentMethodsEnum)(order.PaymentMethod ?? 0),
            TotalAmount = order.TotalAmount,
            TransactionId = order.TransactionId ?? "0"
        };

        await _orderNotificationService.SendOrderNotificationAsync(
            order.AppUser,
            order,
            (OrderStatusEnum)order.OrderStatus,
            (PaymentMethodsEnum)order.PaymentMethod,
            (PaymentStatusEnum)payment.PaymentStatus
        );

        await _unitOfWork.SaveChangesAsync();

        return paymentResponse;
    }
}
