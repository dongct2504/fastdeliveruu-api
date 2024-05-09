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

        if (request.VnPayResponse.VnpayResponseCode != "00")
        {
            vnpayResponse.IsSuccess = false;
            order.OrderStatus = OrderStatus.Cancelled;
            order.PaymentStatus = PaymentStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);

            string message = "Payment was canceled.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<VnpayResponse>(new BadRequestError(message));
        }

        vnpayResponse.IsSuccess = true;
        order.OrderStatus = OrderStatus.Success;
        order.PaymentStatus = OrderStatus.Success;
        await _orderRepository.UpdateAsync(order);

        return vnpayResponse;
    }
}
