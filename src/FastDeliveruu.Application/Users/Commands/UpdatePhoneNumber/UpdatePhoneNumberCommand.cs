using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Users.Commands.UpdatePhoneNumber;

public class UpdatePhoneNumberCommand : IRequest<Result>
{
    public UpdatePhoneNumberCommand(Guid userId, string phoneNumber)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
    }

    public Guid UserId { get; }
    public string PhoneNumber { get; }
}
