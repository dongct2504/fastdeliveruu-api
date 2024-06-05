using FastDeliveruu.Application.Common.Behaviors;
using FluentValidation;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ReceiverName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(ValidateForRequest.BeValidPhoneNumber).WithMessage("Invalid phone number.");

        RuleFor(x => x.TotalAmount)
            .NotEmpty()
            .GreaterThanOrEqualTo(1000);

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .MaximumLength(25)
            .Must(ValidateForRequest.BeValidPaymentMethod).WithMessage("Must be cash, vnpay or momo.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Ward)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.District)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(30);
    }
}
