using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FastDeliveruu.Domain.Specifications.Orders;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Commands.UpdateVnpay;

public class UpdateVnpayCommandHandler : IRequestHandler<UpdateVnpayCommand, Result<VnpayResponse>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateVnpayCommandHandler> _logger;

    public UpdateVnpayCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<UpdateVnpayCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<VnpayResponse>> Handle(UpdateVnpayCommand request, CancellationToken cancellationToken)
    {
        VnpayResponse vnpayResponse = request.VnPayResponse;

        Order? order = await _unitOfWork.Orders.GetWithSpecAsync(new OrderWithPaymentsById(vnpayResponse.OrderId));
        if (order == null)
        {
            string message = "Order not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        Payment? payment = order.Payments.FirstOrDefault();
        if (payment == null)
        {
            string message = "Payment not found for the order.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        order.TransactionId = request.VnPayResponse.TransactionId;

        switch (vnpayResponse.VnpayResponseCode)
        {
            case "00":
                vnpayResponse.IsSuccess = true;
                order.OrderStatus = (byte?)OrderStatusEnum.Success;
                payment.PaymentStatus = (byte?)PaymentStatusEnum.Approved;
                break;

            case "24":
                vnpayResponse.IsSuccess = false;
                order.OrderStatus = (byte?)OrderStatusEnum.Cancelled;
                payment.PaymentStatus = (byte?)PaymentStatusEnum.Cancelled;
                break;

            default:
                vnpayResponse.IsSuccess = false;
                order.OrderStatus = (byte?)OrderStatusEnum.Failed;
                payment.PaymentStatus = (byte?)PaymentStatusEnum.Failed;
                await _unitOfWork.SaveChangesAsync();
                string unknownMessage = $"Payment failed with response code {request.VnPayResponse.VnpayResponseCode}.";
                _logger.LogWarning($"{request.GetType().Name} - {unknownMessage} - {request}");
                return Result.Fail(new BadRequestError(unknownMessage));
        }

        foreach (OrderDetail orderDetail in order.OrderDetails)
        {
            if (!orderDetail.MenuVariantId.HasValue) // menu item
            {
                MenuItemInventory? menuItemInventory = await _unitOfWork.MenuItemInventories
                    .GetWithSpecAsync(new MenuItemInventoryByIdSpecification(orderDetail.MenuItemId));
                if (menuItemInventory == null)
                {
                    string message = "MenuItem Inventory not found.";
                    _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                    return Result.Fail(new BadRequestError(message));
                }

                if (vnpayResponse.IsSuccess)
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
                    .GetWithSpecAsync(new MenuVariantInventoryByIdSpecification(orderDetail.MenuVariantId.Value));
                if (menuVariantInventory == null)
                {
                    string message = "MenuVariant Inventory not found.";
                    _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                    return Result.Fail(new BadRequestError(message));
                }

                if (vnpayResponse.IsSuccess)
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

        await _unitOfWork.SaveChangesAsync();

        return vnpayResponse;
    }
}
