using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result>
{
    public string UserId { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
