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
        try
        {
            GetAllGenresQuery query = new GetAllGenresQuery(page);
            PaginationResponse<GenreDto> getAllGenres = await _mediator.Send(query);

            return Ok(getAllGenres);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id:int}", Name = "GetGenreById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGenreById(int id)
    {
        try
        {
            GetGenreByIdQuery query = new GetGenreByIdQuery(id);
            Result<GenreDetailDto> getGenreResult = await _mediator.Send(query);
            if (getGenreResult.IsFailed)
            {
                return Problem(getGenreResult.Errors);
            }

            return Ok(getGenreResult.Value);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPost]
    // [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(typeof(GenreDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreCommand command)
    {
        try
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
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPut("{id:int}")]
    // [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateGenre(int id, [FromBody] UpdateGenreCommand command)
    {
        try
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
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{id:int}")]
    // [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Staff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        try
        {
            DeleteGenreCommand command = new DeleteGenreCommand(id);
            Result deleteGenreResult = await _mediator.Send(command);
            if (deleteGenreResult.IsFailed)
            {
                return Problem(deleteGenreResult.Errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}
