using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Genres.Commands.DeleteGenre;

public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly IGenreRepository _genreRepository;

    public DeleteGenreCommandHandler(IGenreRepository genreRepository, ICacheService cacheService)
    {
        _genreRepository = genreRepository;
        _cacheService = cacheService;
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

        await _cacheService.RemoveAsync($"{CacheConstants.Genre}-{request.Id}", cancellationToken);

        return Result.Ok();
    }
}