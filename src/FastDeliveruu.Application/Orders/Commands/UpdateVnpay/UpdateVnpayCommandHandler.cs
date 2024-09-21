using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
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

        Order? order = await _unitOfWork.Orders.GetAsync(vnpayResponse.OrderId);
        if (order == null)
        {
            string message = "Order not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<VnpayResponse>(new NotFoundError(message));
        }

        order.TransactionId = request.VnPayResponse.TransactionId;

        switch (vnpayResponse.VnpayResponseCode)
        {
            case "00":
                vnpayResponse.IsSuccess = true;
                order.OrderStatus = (byte?)OrderStatus.Success;
                order.PaymentStatus = (byte?)PaymentStatus.Approved;
                _unitOfWork.Orders.Update(order);
                break;

            case "24":
                vnpayResponse.IsSuccess = false;
                order.OrderStatus = (byte?)OrderStatus.Cancelled;
                order.PaymentStatus = (byte?)PaymentStatus.Cancelled;
                _unitOfWork.Orders.Update(order);
                break;

            default:
                vnpayResponse.IsSuccess = false;
                order.OrderStatus = (byte?)OrderStatus.Failed;
                order.PaymentStatus = (byte?)PaymentStatus.Failed;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                string unknownMessage = $"Payment failed with response code {request.VnPayResponse.VnpayResponseCode}.";
                _logger.LogWarning($"{request.GetType().Name} - {unknownMessage} - {request}");
                return Result.Fail(new BadRequestError(unknownMessage));
        }

        await _unitOfWork.SaveChangesAsync();

        return vnpayResponse;
    }
}
