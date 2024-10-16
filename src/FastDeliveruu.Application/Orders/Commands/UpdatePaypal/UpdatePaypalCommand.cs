using FastDeliveruu.Application.Dtos.PaymentResponses;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.UpdatePaypal;

public class UpdatePaypalCommand : IRequest<Result<PaymentResponse>>
{
    public UpdatePaypalCommand(string token, string payer)
    {
        Token = token;
        PayerId = payer;
    }

    public string Token { get; }
    public string PayerId { get; }
}
