using FastDeliveruu.Application.Dtos.PaymentResponses;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.UpdateVnpay;

public class UpdateVnpayCommand : IRequest<Result<PaymentResponse>>
{
    public UpdateVnpayCommand(PaymentResponse vnPayResponse)
    {
        PaymentResponse = vnPayResponse;
    }

    public PaymentResponse PaymentResponse { get; } = null!;
}
