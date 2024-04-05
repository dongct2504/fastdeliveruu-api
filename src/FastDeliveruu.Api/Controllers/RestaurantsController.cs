using AutoMapper;
using FastDeliveruu.Api.Dtos;
using FastDeliveruu.Api.Interfaces;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Common;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsController : ControllerBase
{
    private readonly ApiResponse _apiResponse;
    private readonly IRestaurantServices _restaurantServices;
    private readonly ILogger<RestaurantsController> _logger;
    private readonly IMapper _mapper;
    private readonly IImageServices _imageServices;

    public RestaurantsController(IRestaurantServices restaurantServices,
        ILogger<RestaurantsController> logger,
        IMapper mapper,
        IImageServices imageServices)
    {
        _apiResponse = new ApiResponse();
        _restaurantServices = restaurantServices;
        _logger = logger;
        _mapper = mapper;
        _imageServices = imageServices;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetAllRestaurants()
    {
        try
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<IEnumerable<RestaurantDto>>(
                await _restaurantServices.GetAllRestaurantsAsync());

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

    [HttpGet("{id:int}", Name = "GetRestaurantById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetRestaurantById(int id)
    {
        try
        {
            Restaurant? restaurant = await _restaurantServices.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                string errorMessage = $"Restaurant not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = await _restaurantServices.GetRestaurantWithMenuItemsByIdAsync(id);

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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateRestaurant(
        [FromForm] RestaurantCreateWithImageDto restaurantCreateWithImageDto)
    {
        try
        {
            if (restaurantCreateWithImageDto.RestaurantCreateDto == null)
            {
                string errorMessage = "Can't create the requested restaurant because it is null.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            if (await _restaurantServices.GetRestaurantByNameAsync(
                restaurantCreateWithImageDto.RestaurantCreateDto.Name) != null)
            {
                string errorMessage = "Can't create the requested restaurant because it already exists.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            Restaurant restaurant = _mapper.Map<Restaurant>(restaurantCreateWithImageDto.RestaurantCreateDto);

            if (restaurantCreateWithImageDto.ImageFile != null)
            {
                // create and save image
                string uploadImagePath = @"images\restaurants";
                string? fileNameWithExtension = await _imageServices.UploadImageAsync(
                    restaurantCreateWithImageDto.ImageFile, uploadImagePath);
                restaurant.ImageUrl = @"\images\restaurants\" + fileNameWithExtension;
            }

            restaurant.CreatedAt = DateTime.Now;
            restaurant.UpdatedAt = DateTime.Now;

            int createdRestaurantId = await _restaurantServices.CreateRestaurantAsync(restaurant);
            restaurant.RestaurantId = createdRestaurantId;

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<RestaurantDto>(restaurant);

            return CreatedAtRoute(nameof(GetRestaurantById), new { Id = createdRestaurantId }, _apiResponse);
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> UpdateRestaurant(int id,
        [FromForm] RestaurantUpdateWithImageDto restaurantUpdateWithImageDto)
    {
        try
        {
            Restaurant? restaurant = await _restaurantServices.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                string errorMessage = $"Restaurant not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            _mapper.Map(restaurantUpdateWithImageDto.RestaurantUpdateDto, restaurant);

            if (restaurantUpdateWithImageDto.ImageFile != null)
            {
                _imageServices.DeleteImage(restaurant.ImageUrl); // delete the old ones if it already exist

                string uploadImagePath = @"images\restaurants";
                string? fileNameWithExtension = await _imageServices.UploadImageAsync(
                    restaurantUpdateWithImageDto.ImageFile, uploadImagePath);
                restaurant.ImageUrl = @"\images\restaurants\" + fileNameWithExtension;
            }

            restaurant.UpdatedAt = DateTime.Now;

            await _restaurantServices.UpdateRestaurantAsync(restaurant);

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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> DeleteRestaurant(int id)
    {
        try
        {
            Restaurant? restaurant = await _restaurantServices.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                string errorMessage = $"Restaurant not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            await _restaurantServices.DeleteRestaurantAsync(id);
            _imageServices.DeleteImage(restaurant.ImageUrl);

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