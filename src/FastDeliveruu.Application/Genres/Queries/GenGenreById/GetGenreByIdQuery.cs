using FastDeliveruu.Application.Dtos.GenreDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Genres.Queries.GenGenreById;

public class GetGenreByIdQuery : IRequest<Result<GenreDetailDto>>
{
    public GetGenreByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }
}