using FastDeliveruu.Application.Common.Behaviors;
using FastDeliveruu.Common.Constants;
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
            .Must(ValidateForRequest.BeValidPaymentMethod)
            .WithMessage(ErrorMessageConstants.DeliveryValidator);

        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.StreetName)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.WardId)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.CityId)
            .NotEmpty();
    }
}
