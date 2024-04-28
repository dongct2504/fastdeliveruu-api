using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQuery : IRequest<PaginationResponse<GenreDto>>
{
    public GetAllGenresQuery(int pageNumber)
    {
        PageNumber = pageNumber;
    }

    public int PageNumber { get; }
}