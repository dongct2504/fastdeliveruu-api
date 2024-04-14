using AutoMapper;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Common.Roles;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/genres")]
public class GenresController : ControllerBase
{
    private readonly IGenreServices _genreServices;
    private readonly ILogger<GenresController> _logger;
    private readonly IMapper _mapper;

    public GenresController(IGenreServices genreServices,
        ILogger<GenresController> logger,
        IMapper mapper)
    {
        _genreServices = genreServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = "Default30")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllGenres()
    {
        try
        {
            return Ok(_mapper.Map<IEnumerable<GenreDto>>(await _genreServices.GetAllGenresAsync()));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id}", Name = "GetGenreById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGenreById(int id)
    {
        try
        {
            Result<Genre> getGenreResult = await _genreServices.GetGenreWithMenuItemsByIdAsync(id);
            if (getGenreResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: getGenreResult.Errors[0].Message);
            }

            return Ok(_mapper.Map<GenreDetailDto>(getGenreResult.Value));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.RoleAdmin + "," + RoleConstants.RoleStaff)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateGenre([FromBody] GenreCreateDto genreCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Genre genre = _mapper.Map<Genre>(genreCreateDto);
            genre.CreatedAt = DateTime.Now;
            genre.UpdatedAt = DateTime.Now;

            Result<int> genreResult = await _genreServices.CreateGenreAsync(genre);
            if (genreResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status409Conflict,
                    detail: genreResult.Errors[0].Message);
            }

            genre.GenreId = genreResult.Value;
            GenreDto genreDto = _mapper.Map<GenreDto>(genre);

            return CreatedAtRoute(nameof(GetGenreById), new { Id = genreDto.GenreId }, genreDto);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.RoleAdmin + "," + RoleConstants.RoleStaff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreUpdateDto genreUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Genre genre = _mapper.Map<Genre>(genreUpdateDto);
            genre.UpdatedAt = DateTime.Now;

            Result genreResult = await _genreServices.UpdateGenreAsync(id, genre);
            if (genreResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: genreResult.Errors[0].Message);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        try
        {
            Result genreResult = await _genreServices.DeleteGenreAsync(id);
            if (genreResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: genreResult.Errors[0].Message);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}
