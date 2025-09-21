using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Users.Commands.ToggleLock;

public class ToggleLockCommand : IRequest<Result>
{
    public ToggleLockCommand(string userName, string action)
    {
        UserName = userName;
        Action = action;
    }

    public string UserName { get; set; } = null!;
    public string Action { get; set; } = null!;
}
