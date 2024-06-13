using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Orders.Commands.UpdateVnpay;

public class UpdateVnpayCommandHandler : IRequestHandler<UpdateVnpayCommand, Result<VnpayResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateVnpayCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<VnpayResponse>> Handle(UpdateVnpayCommand request, CancellationToken cancellationToken)
    {
        VnpayResponse vnpayResponse = request.VnPayResponse;

        Order? order = await _orderRepository.GetAsync(vnpayResponse.OrderId);
        if (order == null)
        {
            string message = "Order not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<VnpayResponse>(new NotFoundError(message));
        }

        order.TransactionId = request.VnPayResponse.TransactionId;

        switch (vnpayResponse.VnpayResponseCode)
        {
            case "00":
                vnpayResponse.IsSuccess = true;
                order.OrderStatus = OrderStatus.Success;
                order.PaymentStatus = PaymentStatus.Approved;
                await _orderRepository.UpdateAsync(order);
                return vnpayResponse;

            case "24":
                vnpayResponse.IsSuccess = false;
                order.OrderStatus = OrderStatus.Cancelled;
                order.PaymentStatus = PaymentStatus.Cancelled;
                await _orderRepository.UpdateAsync(order);
                string cancelMessage = "Payment was canceled.";
                Log.Warning($"{request.GetType().Name} - {cancelMessage} - {request}");
                return Result.Fail(new BadRequestError(cancelMessage));

            case "51":
                vnpayResponse.IsSuccess = false;
                order.OrderStatus = OrderStatus.Failed;
                order.PaymentStatus = PaymentStatus.Failed;
                await _orderRepository.UpdateAsync(order);
                string failedMessage = "Payment failed due to insufficient funds.";
                Log.Warning($"{request.GetType().Name} - {failedMessage} - {request}");
                return Result.Fail(new BadRequestError(failedMessage));

            default:
                vnpayResponse.IsSuccess = false;
                order.OrderStatus = OrderStatus.Failed;
                order.PaymentStatus = PaymentStatus.Failed;
                await _orderRepository.UpdateAsync(order);
                string unknownMessage = $"Payment failed with response code {request.VnPayResponse.VnpayResponseCode}.";
                Log.Warning($"{request.GetType().Name} - {unknownMessage} - {request}");
                return Result.Fail(new BadRequestError(unknownMessage));
        }
    }
}
