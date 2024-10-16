using FastDeliveruu.Application.Common.Behaviors;
using FluentValidation;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.DeliveryMethodId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .Must(ValidateForRequest.BeValidPaymentMethod).WithMessage("Must be cash, vnpay, paypal or momo.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.WardId)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.CityId)
            .NotEmpty();
    }
}
