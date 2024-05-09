using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Genres.Commands.DeleteGenre;

public class DeleteGenreCommand : IRequest<Result>
{
    public DeleteGenreCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}