using FluentValidation;

namespace FastDeliveruu.Application.Wards.Commands.UpdateWard;

public class UpdateWardCommandValidator : AbstractValidator<UpdateWardCommand>
{
    public UpdateWardCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}
