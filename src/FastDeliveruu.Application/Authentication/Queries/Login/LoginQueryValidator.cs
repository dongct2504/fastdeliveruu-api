using FluentValidation;

namespace FastDeliveruu.Application.Authentication.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}