using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Common;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiController]
[Route("api/genres")]
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
    public async Task<ActionResult<ApiResponse>> GetAllGenres()
    {
        try
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<IEnumerable<GenreDto>>(await _genreServices.GetAllGenresAsync());

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpGet("{id:int}", Name = "GetGenreById")]
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
            _apiResponse.Result = _mapper.Map<GenreDto>(genre);

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateGenre(GenreCreateDto genreCreateDto)
    {
        try
        {
            if (await _genreServices.GetGenreByNameAsync(genreCreateDto.Name) != null)
            {
                string errorMessage = "Can't create the requested genre because it already exists.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            if (genreCreateDto == null)
            {
                string errorMessage = "Can't create the requested genre because it is null.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            Genre genre = _mapper.Map<Genre>(genreCreateDto);
            genre.CreatedAt = DateTime.Now;
            genre.UpdatedAt = DateTime.Now;

            int createdGenreId = await _genreServices.CreateGenreAsync(genre);
            genre.GenreId = createdGenreId;

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<GenreDto>(genre);

            return CreatedAtRoute(nameof(GetGenreById), new { Id = createdGenreId }, _apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse>> UpdateGenre(int id,
        GenreUpdateDto genreUpdateDto)
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

            // Source -> Target
            _mapper.Map(genreUpdateDto, genre);
            genre.UpdatedAt = DateTime.Now;

            await _genreServices.UpdateGenreAsync(genre);

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpDelete("{id:int}")]
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

            await _genreServices.RemoveGenreAsync(id);

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.Message };

            return StatusCode(500, _apiResponse);
        }
    }
}
