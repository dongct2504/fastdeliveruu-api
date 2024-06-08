using FastDeliveruu.Application.Dtos.GenreDtos;
using MediatR;

namespace FastDeliveruu.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQuery : IRequest<List<GenreDto>>
{
    public GetAllGenresQuery(int pageNumber)
    {
        PageNumber = pageNumber;
    }

    public int PageNumber { get; }
}