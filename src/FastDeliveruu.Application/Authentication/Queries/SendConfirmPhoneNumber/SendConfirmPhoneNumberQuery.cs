using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.SendConfirmPhoneNumber;

public class SendConfirmPhoneNumberQuery : IRequest<Result>
{
    public SendConfirmPhoneNumberQuery(Guid userId, string phoneNumber)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
    }

    public Guid UserId { get; }
    public string PhoneNumber { get; }
}
