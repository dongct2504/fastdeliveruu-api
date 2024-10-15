using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.ConfirmPhoneNumber;

public class ConfirmPhoneNumberQuery : IRequest<Result>
{
    public ConfirmPhoneNumberQuery(Guid userId, string otpCode)
    {
        UserId = userId;
        OtpCode = otpCode;
    }

    public Guid UserId { get; }
    public string OtpCode { get; }
}
