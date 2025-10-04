using FastDeliveruu.Common.Constants;
using FastDeliveruu.Common.Enums;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FastDeliveruu.Domain.Specifications.Orders;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Commands.UpdateVnpay;

public class UpdateVnpayCommandHandler : IRequestHandler<UpdateVnpayCommand, Result<PaymentResponse>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IMailNotificationService _orderNotificationService;
    private readonly ILogger<UpdateVnpayCommandHandler> _logger;

    public UpdateVnpayCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<UpdateVnpayCommandHandler> logger, IMailNotificationService orderNotificationService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _orderNotificationService = orderNotificationService;
    }

    public async Task<Result<PaymentResponse>> Handle(UpdateVnpayCommand request, CancellationToken cancellationToken)
    {
        PaymentResponse paymentResponse = request.PaymentResponse;

        Order? order = await _unitOfWork.Orders.GetWithSpecAsync(new OrderWithPaymentsById(paymentResponse.OrderId));
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

        order.TransactionId = request.PaymentResponse.TransactionId;
        payment.TransactionId = request.PaymentResponse.TransactionId;

        switch (paymentResponse.VnpayResponseCode)
        {
            case "00":
                paymentResponse.IsSuccess = true;
                order.OrderStatus = (byte?)OrderStatusEnum.Success;
                payment.PaymentStatus = (byte?)PaymentStatusEnum.Approved;
                break;

            case "24":
                paymentResponse.IsSuccess = false;
                order.OrderStatus = (byte?)OrderStatusEnum.Cancelled;
                payment.PaymentStatus = (byte?)PaymentStatusEnum.Cancelled;
                break;

            default:
                paymentResponse.IsSuccess = false;
                order.OrderStatus = (byte?)OrderStatusEnum.Failed;
                payment.PaymentStatus = (byte?)PaymentStatusEnum.Failed;
                await _unitOfWork.SaveChangesAsync();

                string unknownMessage = $"Payment failed with response code {request.PaymentResponse.VnpayResponseCode}.";
                _logger.LogWarning($"{request.GetType().Name} - {unknownMessage} - {request}");
                return Result.Fail(new BadRequestError(unknownMessage));
        }

        foreach (OrderDetail orderDetail in order.OrderDetails)
        {
            if (!orderDetail.MenuVariantId.HasValue) // menu item
            {
                MenuItemInventory? menuItemInventory = await _unitOfWork.MenuItemInventories
                    .GetWithSpecAsync(new MenuItemInventoryByMenuItemIdSpecification(orderDetail.MenuItemId));
                if (menuItemInventory == null)
                {
                    _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemInventoryNotFound} - {request}");
                    return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemInventoryNotFound));
                }

                if (paymentResponse.IsSuccess)
                {
                    menuItemInventory.QuantityReserved -= orderDetail.Quantity;
                }
                else
                {
                    menuItemInventory.QuantityAvailable += orderDetail.Quantity;
                    menuItemInventory.QuantityReserved -= orderDetail.Quantity;
                }
            }
            else // menu variant
            {
                MenuVariantInventory? menuVariantInventory = await _unitOfWork.MenuVariantInventories
                    .GetWithSpecAsync(new MenuVariantInventoryByMenuVariantIdSpecification(orderDetail.MenuVariantId.Value));
                if (menuVariantInventory == null)
                {
                    _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuVariantInventoryNotFound} - {request}");
                    return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuVariantInventoryNotFound));
                }

                if (paymentResponse.IsSuccess)
                {
                    menuVariantInventory.QuantityReserved -= orderDetail.Quantity;
                }
                else
                {
                    menuVariantInventory.QuantityAvailable += orderDetail.Quantity;
                    menuVariantInventory.QuantityReserved -= orderDetail.Quantity;
                }
            }
        }

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
