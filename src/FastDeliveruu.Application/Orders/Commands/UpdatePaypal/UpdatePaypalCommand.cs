using FastDeliveruu.Application.Common.Helpers;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.UpdatePaypal;

public class UpdatePaypalCommand : IRequest<Result<PaymentResponse>>
{
    public UpdatePaypalCommand(CaptureOrderResponse captureOrderResponse)
    {
        CaptureOrderResponse = captureOrderResponse;
    }

    public CaptureOrderResponse CaptureOrderResponse { get; }
}
