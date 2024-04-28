using FastDeliveruu.Application.Dtos.GenreDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Genres.Commands.CreateGenre;

public class CreateGenreCommand : IRequest<Result<GenreDto>>
{
    public string Name { get; set; } = null!;
}