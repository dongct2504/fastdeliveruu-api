using FluentValidation;

namespace FastDeliveruu.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .MaximumLength(50);

        RuleFor(x => x.Address)
            .NotNull()
            .MaximumLength(60);

        RuleFor(x => x.CityId)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.WardId)
            .NotEmpty();
    }
}