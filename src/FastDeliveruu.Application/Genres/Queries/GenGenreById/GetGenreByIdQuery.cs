using FastDeliveruu.Application.Dtos.GenreDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Genres.Queries.GenGenreById;

public class GetGenreByIdQuery : IRequest<Result<GenreDetailDto>>
{
    public GetGenreByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}