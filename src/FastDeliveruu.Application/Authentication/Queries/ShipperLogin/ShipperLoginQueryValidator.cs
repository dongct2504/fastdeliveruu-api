using FluentValidation;

namespace FastDeliveruu.Application.Authentication.Queries.ShipperLogin;

public class ShipperLoginQueryValidator : AbstractValidator<ShipperLoginQuery>
{
    public ShipperLoginQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
