using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Common.Roles;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MapsterMapper;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/genres")]
public class GenresController : ApiController
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
                return Problem(getGenreResult.Errors);
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

            Result<int> createGenreResult = await _genreServices.CreateGenreAsync(genre);
            if (createGenreResult.IsFailed)
            {
                return Problem(createGenreResult.Errors);
            }

            genre.GenreId = createGenreResult.Value;
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

            Result<Genre> getGenreResult = await _genreServices.GetGenreByIdAsync(id);
            if (getGenreResult.IsFailed)
            {
                return Problem(getGenreResult.Errors);
            }

            Genre genre = getGenreResult.Value;

            _mapper.Map(genreUpdateDto, genre);

            genre.UpdatedAt = DateTime.Now;

            Result updateGenreResult = await _genreServices.UpdateGenreAsync(id, genre);
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

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.RoleAdmin + "," + RoleConstants.RoleStaff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        try
        {
            Result deleteGenreResult = await _genreServices.DeleteGenreAsync(id);
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
