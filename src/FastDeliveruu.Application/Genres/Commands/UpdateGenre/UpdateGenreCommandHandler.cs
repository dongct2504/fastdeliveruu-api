using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Genres.Commands.UpdateGenre;

public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, Result>
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public UpdateGenreCommandHandler(IGenreRepository genreRepository, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        Genre? genre = await _genreRepository.GetAsync(request.GenreId);
        if (genre == null)
        {
            string message = "Genre not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, genre);
        genre.UpdatedAt = DateTime.Now;

        await _genreRepository.UpdateAsync(genre);

        return Result.Ok();
    }
}