using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/genres")]
public class GenresController : ControllerBase
{
    private readonly ApiResponse _apiResponse;
    private readonly IGenreServices _genreServices;
    private readonly ILogger<GenresController> _logger;
    private readonly IMapper _mapper;

    public GenresController(IGenreServices genreServices,
        ILogger<GenresController> logger,
        IMapper mapper)
    {
        _apiResponse = new ApiResponse();
        _genreServices = genreServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = "Default30", VaryByQueryKeys = new[] { "page" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetAllGenres(int page = 1)
    {
        try
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<IEnumerable<GenreDto>>(
                await _genreServices.GetAllGenresAsync(page));

            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpGet("{id:int}", Name = "GetGenreById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetGenreById(int id)
    {
        try
        {
            Genre? genre = await _genreServices.GetGenreByIdAsync(id);
            if (genre == null)
            {
                string errorMessage = $"Genre not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = await _genreServices.GetGenreWithMenuItemsByIdAsync(id);

            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateGenre([FromBody] GenreCreateDto genreCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (genreCreateDto == null)
            {
                string errorMessage = "Can't create the requested genre because it is null.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            Genre? genre = await _genreServices.GetGenreByNameAsync(genreCreateDto.Name);
            if (genre != null)
            {
                string errorMessage = "Can't create the requested genre because it already exists.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Conflict;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return Conflict(_apiResponse);
            }

            genre = _mapper.Map<Genre>(genreCreateDto);
            genre.CreatedAt = DateTime.Now;
            genre.UpdatedAt = DateTime.Now;

            int createdGenreId = await _genreServices.CreateGenreAsync(genre);
            genre.GenreId = createdGenreId;

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<GenreDto>(genre);

            return CreatedAtRoute(nameof(GetGenreById), new { Id = createdGenreId }, _apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> UpdateGenre(int id,
        [FromBody] GenreUpdateDto genreUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Genre? genre = await _genreServices.GetGenreByIdAsync(id);
            if (genre == null)
            {
                string errorMessage = $"Genre not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            // Source -> Target
            _mapper.Map(genreUpdateDto, genre);
            genre.UpdatedAt = DateTime.Now;

            await _genreServices.UpdateGenreAsync(genre);

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> DeleteGenre(int id)
    {
        try
        {
            Genre? genre = await _genreServices.GetGenreByIdAsync(id);
            if (genre == null)
            {
                string errorMessage = $"Genre not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            await _genreServices.DeleteGenreAsync(id);

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _apiResponse);
        }
    }
}
