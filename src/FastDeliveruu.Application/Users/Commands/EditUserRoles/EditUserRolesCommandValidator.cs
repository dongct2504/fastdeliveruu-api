using FluentValidation;

namespace FastDeliveruu.Application.Users.Commands.EditUserRoles;

public class EditUserRolesCommandValidator : AbstractValidator<EditUserRolesCommand>
{
    public EditUserRolesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Roles)
            .NotEmpty();
    }
}
