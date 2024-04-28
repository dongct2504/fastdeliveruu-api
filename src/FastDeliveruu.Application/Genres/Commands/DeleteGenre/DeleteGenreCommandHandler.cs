using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Genres.Commands.DeleteGenre;

public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand, Result>
{
    private readonly IGenreRepository _genreRepository;

    public DeleteGenreCommandHandler(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<Result> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        Genre? genre = await _genreRepository.GetAsync(request.Id);
        if (genre == null)
        {
            string message = "Genre not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        await _genreRepository.DeleteAsync(genre);

        return Result.Ok();
    }
}