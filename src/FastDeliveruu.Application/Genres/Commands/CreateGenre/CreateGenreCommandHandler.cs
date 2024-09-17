using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Genres;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Genres.Commands.CreateGenre;

public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, Result<GenreDto>>
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public CreateGenreCommandHandler(IGenreRepository genreRepository, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<Result<GenreDto>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var spec = new GenreByNameSpecification(request.Name);
        Genre? genre = await _genreRepository.GetWithSpecAsync(spec, asNoTracking: true);
        if (genre != null)
        {
            string message = "genre is already exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<GenreDto>(new DuplicateError(message));
        }

        genre = _mapper.Map<Genre>(request);
        genre.Id = Guid.NewGuid();
        genre.CreatedAt = DateTime.Now;
        genre.UpdatedAt = DateTime.Now;

        await _genreRepository.AddAsync(genre);

        return _mapper.Map<GenreDto>(genre);
    }
}