using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Common.Constants;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FastDeliveruu.Application.Genres.Queries.GetAllGenres;
using FastDeliveruu.Application.Dtos;
using MediatR;
using FastDeliveruu.Application.Genres.Queries.GenGenreById;
using FastDeliveruu.Application.Genres.Commands.CreateGenre;
using FastDeliveruu.Application.Genres.Commands.UpdateGenre;
using FastDeliveruu.Application.Genres.Commands.DeleteGenre;
using Asp.Versioning;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/genres")]
public class GenresController : ApiController
{
    private readonly IMediator _mediator;

    public GenresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = CacheProfileConstants.Default30, VaryByQueryKeys = new[] { "page" })]
    [ProducesResponseType(typeof(PaginationResponse<GenreDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGenres(int page = 1)
    {
        GetAllGenresQuery query = new GetAllGenresQuery(page);
        PaginationResponse<GenreDto> getAllGenres = await _mediator.Send(query);

        return Ok(getAllGenres);
    }

    [HttpGet("{id:guid}", Name = "GetGenreById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGenreById(Guid id)
    {
        GetGenreByIdQuery query = new GetGenreByIdQuery(id);
        Result<GenreDetailDto> getGenreResult = await _mediator.Send(query);
        if (getGenreResult.IsFailed)
        {
            return Problem(getGenreResult.Errors);
        }

        return Ok(getGenreResult.Value);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(typeof(GenreDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreCommand command)
    {
        Result<GenreDto> createGenreResult = await _mediator.Send(command);
        if (createGenreResult.IsFailed)
        {
            return Problem(createGenreResult.Errors);
        }

        return CreatedAtRoute(
            nameof(GetGenreById),
            new { Id = createGenreResult.Value.GenreId },
            createGenreResult.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateGenre(Guid id, [FromBody] UpdateGenreCommand command)
    {
        if (id != command.GenreId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result updateGenreResult = await _mediator.Send(command);
        if (updateGenreResult.IsFailed)
        {
            return Problem(updateGenreResult.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        DeleteGenreCommand command = new DeleteGenreCommand(id);
        Result deleteGenreResult = await _mediator.Send(command);
        if (deleteGenreResult.IsFailed)
        {
            return Problem(deleteGenreResult.Errors);
        }

        return NoContent();
    }
}
