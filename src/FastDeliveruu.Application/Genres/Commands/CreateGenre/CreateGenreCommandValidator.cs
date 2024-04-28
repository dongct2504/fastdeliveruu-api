using FluentValidation;

namespace FastDeliveruu.Application.Genres.Commands.CreateGenre;

public class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
{
    public CreateGenreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(126);
    }
}