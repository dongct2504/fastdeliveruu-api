using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Genres.Commands.UpdateGenre;

public class UpdateGenreCommand : IRequest<Result>
{
    public Guid GenreId { get; set; }

    public string Name { get; set; } = null!;
}