using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Genres.Queries.GenGenreById;

public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, Result<GenreDetailDto>>
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public GetGenreByIdQueryHandler(IGenreRepository genreRepository, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<Result<GenreDetailDto>> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
    {
        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            SetIncludes = "MenuItems",
            Where = g => g.GenreId == request.Id
        };
        Genre? genre = await _genreRepository.GetAsync(options);
        if (genre == null)
        {
            string message = "Genre not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<GenreDetailDto>(new NotFoundError(message));
        }

        return _mapper.Map<GenreDetailDto>(genre);
    }
}