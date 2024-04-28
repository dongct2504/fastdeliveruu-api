using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Genres.Commands.DeleteGenre;

public class DeleteGenreCommand : IRequest<Result>
{
    public DeleteGenreCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}