using AutoMapper;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Common.Roles;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/restaurants")]
public class RestaurantsController : ApiController
{
    private readonly IRestaurantServices _restaurantServices;
    private readonly ILogger<RestaurantsController> _logger;
    private readonly IMapper _mapper;
    private readonly IImageServices _imageServices;

    public RestaurantsController(IRestaurantServices restaurantServices,
        ILogger<RestaurantsController> logger,
        IMapper mapper,
        IImageServices imageServices)
    {
        _restaurantServices = restaurantServices;
        _logger = logger;
        _mapper = mapper;
        _imageServices = imageServices;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = "Default30")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllRestaurants()
    {
        try
        {
            return Ok(_mapper.Map<IEnumerable<RestaurantDto>>(
                await _restaurantServices.GetAllRestaurantsAsync()));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id}", Name = "GetRestaurantById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRestaurantById(Guid id)
    {
        try
        {
            Result<Restaurant> getRestaurantResult =
                await _restaurantServices.GetRestaurantWithMenuItemsByIdAsync(id);
            if (getRestaurantResult.IsFailed)
            {
                return Problem(getRestaurantResult.Errors);
            }

            return Ok(_mapper.Map<RestaurantDetailDto>(getRestaurantResult.Value));
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRestaurant([FromForm] RestaurantCreateDto restaurantCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Restaurant restaurant = _mapper.Map<Restaurant>(restaurantCreateDto);
            if (restaurantCreateDto.ImageFile != null)
            {
                // create and save image
                string uploadImagePath = @"images\restaurants";
                string? fileNameWithExtension = await _imageServices.UploadImageAsync(
                    restaurantCreateDto.ImageFile, uploadImagePath);
                restaurant.ImageUrl = @"\images\restaurants\" + fileNameWithExtension;
            }
            restaurant.CreatedAt = DateTime.Now;
            restaurant.UpdatedAt = DateTime.Now;

            Result<Guid> createRestaurantResult = await _restaurantServices.CreateRestaurantAsync(restaurant);
            if (createRestaurantResult.IsFailed)
            {
                await _imageServices.DeleteImage(restaurant.ImageUrl);

                return Problem(createRestaurantResult.Errors);
            }

            restaurant.RestaurantId = createRestaurantResult.Value;

            RestaurantDto restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

            return CreatedAtRoute(nameof(GetRestaurantById),
                new { Id = restaurant.RestaurantId }, restaurantDto);
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
    public async Task<IActionResult> UpdateRestaurant(Guid id,
        [FromForm] RestaurantUpdateDto restaurantUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<Restaurant> oldRestaurantResult = await _restaurantServices.GetRestaurantByIdAsync(id);
            if (oldRestaurantResult.IsFailed)
            {
                return Problem(oldRestaurantResult.Errors);
            }

            Restaurant restaurant = oldRestaurantResult.Value;
            string? oldImagePath = oldRestaurantResult.Value.ImageUrl;

            _mapper.Map(restaurantUpdateDto, restaurant);

            if (restaurantUpdateDto.ImageFile != null)
            {
                string uploadImagePath = @"images\restaurants";
                string? fileNameWithExtension = await _imageServices.UploadImageAsync(
                    restaurantUpdateDto.ImageFile, uploadImagePath);
                restaurant.ImageUrl = @"\images\restaurants\" + fileNameWithExtension;
            }
            restaurant.UpdatedAt = DateTime.Now;

            Result updateRestaurantResult = await _restaurantServices.UpdateRestaurantAsync(id, restaurant);
            if (updateRestaurantResult.IsFailed)
            {
                if (restaurantUpdateDto.ImageFile != null)
                {
                    await _imageServices.DeleteImage(restaurant.ImageUrl);
                }

                return Problem(updateRestaurantResult.Errors);
            }

            if (restaurantUpdateDto.ImageFile != null)
            {
                // if it has an old one, delete it after successfully updated
                await _imageServices.DeleteImage(oldImagePath);
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
    public async Task<IActionResult> DeleteRestaurant(Guid id)
    {
        try
        {
            Result<Restaurant> restaurantDeleteResult = await _restaurantServices.GetRestaurantByIdAsync(id);
            if (restaurantDeleteResult.IsFailed)
            {
                return Problem(restaurantDeleteResult.Errors);
            }

            await _restaurantServices.DeleteRestaurantAsync(id);

            await _imageServices.DeleteImage(restaurantDeleteResult.Value.ImageUrl);

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}