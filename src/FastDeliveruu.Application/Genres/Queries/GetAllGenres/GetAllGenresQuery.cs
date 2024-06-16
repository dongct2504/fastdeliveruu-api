using FastDeliveruu.Application.Dtos.GenreDtos;
using MediatR;

namespace FastDeliveruu.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQuery : IRequest<List<GenreDto>>
{
}