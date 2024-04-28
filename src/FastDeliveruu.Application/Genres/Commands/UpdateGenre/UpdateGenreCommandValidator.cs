using FluentValidation;

namespace FastDeliveruu.Application.Genres.Commands.UpdateGenre;

public class UpdateGenreCommandValidator : AbstractValidator<UpdateGenreCommand>
{
    public UpdateGenreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(126);
    }
}