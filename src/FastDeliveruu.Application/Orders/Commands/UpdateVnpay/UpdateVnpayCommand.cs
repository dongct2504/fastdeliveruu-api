using FastDeliveruu.Application.Dtos.PaymentResponses;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.UpdateVnpay;

public class UpdateVnpayCommand : IRequest<Result<VnpayResponse>>
{
    public UpdateVnpayCommand(VnpayResponse vnPayResponse)
    {
        VnPayResponse = vnPayResponse;
    }

    public VnpayResponse VnPayResponse { get; } = null!;
}
