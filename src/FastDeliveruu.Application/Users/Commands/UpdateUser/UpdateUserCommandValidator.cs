using FastDeliveruu.Application.Common.ValidationConfigs;
using FluentValidation;

namespace FastDeliveruu.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(ValidateForRequest.ValidPhoneNumber).WithMessage("Invalid phone number.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(128);
    }
}