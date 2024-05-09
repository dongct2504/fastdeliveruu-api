using FastDeliveruu.Application.Common.Behaviors;
using FluentValidation;

namespace FastDeliveruu.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.LocalUserId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.FirstName)
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(ValidateForRequest.BeValidPhoneNumber).WithMessage("Invalid phone number.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(128);
    }
}