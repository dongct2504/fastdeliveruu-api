using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Result>
{
    public string? UserId { get; set; }
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
