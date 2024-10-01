using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Users.Commands.EditUserRoles;

public class EditUserRolesCommand : IRequest<Result<string[]>>
{
    public EditUserRolesCommand(Guid id, string roles)
    {
        Id = id;
        Roles = roles;
    }

    public Guid Id { get; }
    public string Roles { get; }
}
