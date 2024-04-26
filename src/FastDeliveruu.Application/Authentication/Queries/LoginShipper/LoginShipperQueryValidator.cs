using FluentValidation;

namespace FastDeliveruu.Application.Authentication.Queries.LoginShipper;

public class LoginShipperQueryValidator : AbstractValidator<LoginShipperQuery>
{
    public LoginShipperQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}