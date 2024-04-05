using AutoMapper;
using FastDeliveruu.Api.Dtos;
using FastDeliveruu.Api.Interfaces;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Common;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu-items")]
public class MenuItemController : ControllerBase
{
    private readonly ApiResponse _apiResponse;
    private readonly IMenuItemServices _menuItemServices;
    private readonly ILogger<MenuItemController> _logger;
    private readonly IMapper _mapper;
    private readonly IImageServices _imageServices;

    public MenuItemController(IMenuItemServices menuItemServices,
        ILogger<MenuItemController> logger,
        IMapper mapper,
        IImageServices imageServices)
    {
        _apiResponse = new ApiResponse();
        _menuItemServices = menuItemServices;
        _logger = logger;
        _mapper = mapper;
        _imageServices = imageServices;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = "Default30")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetAllMenuItems()
    {
        try
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = await _menuItemServices.GetAllMenuItemsWithRestaurantGenreAsync();

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

    [HttpGet("{id:int}", Name = "GetMenuItemById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetMenuItemById(int id)
    {
        try
        {
            MenuItem? menuItem = await _menuItemServices.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                string errorMessage = $"MenuItem not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = await _menuItemServices.GetMenuItemWithRestaurantGenreByIdAsync(id);

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
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateMenuItem(
        [FromForm] MenuItemCreateWithImageDto menuItemCreateWithImageDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (menuItemCreateWithImageDto == null)
            {
                string errorMessage = "Can't create the requested menu item because it is null.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            if (await _menuItemServices.GetMenuItemByNameAsync(
                menuItemCreateWithImageDto.MenuItemCreateDto.Name) != null)
            {
                string errorMessage = "Can't create the requested menu item because it already exists.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            MenuItem menuItem = _mapper.Map<MenuItem>(menuItemCreateWithImageDto.MenuItemCreateDto);

            // create and save image
            if (menuItemCreateWithImageDto.ImageFile != null)
            {
                string uploadImagePath = @"images\menu-items";
                string? fileNameWithExtension =
                    await _imageServices.UploadImageAsync(menuItemCreateWithImageDto.ImageFile, uploadImagePath);
                menuItem.ImageUrl = @"\images\menu-items\" + fileNameWithExtension;
            }

            menuItem.CreatedAt = DateTime.Now;
            menuItem.UpdatedAt = DateTime.Now;

            int createdMenuItemId = await _menuItemServices.CreateMenuItemAsync(menuItem);
            menuItem.MenuItemId = createdMenuItemId;

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<MenuItemDto>(menuItem);

            return CreatedAtRoute(nameof(GetMenuItemById), new { Id = createdMenuItemId }, _apiResponse);
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
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id,
        [FromForm] MenuItemUpdateWithImageDto menuItemUpdateWithImageDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MenuItem? menuItem = await _menuItemServices.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                string errorMessage = $"Menu item not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            _mapper.Map(menuItemUpdateWithImageDto.MenuItemUpdateDto, menuItem);

            if (menuItemUpdateWithImageDto.ImageFile != null)
            {
                _imageServices.DeleteImage(menuItem.ImageUrl); // if it has an old one, delete it

                string uploadImagePath = @"images\menu-items";
                string? fileNameWithExtension =
                    await _imageServices.UploadImageAsync(menuItemUpdateWithImageDto.ImageFile, uploadImagePath);
                menuItem.ImageUrl = @"\images\menu-items\" + fileNameWithExtension;
            }

            menuItem.UpdatedAt = DateTime.Now;

            await _menuItemServices.UpdateMenuItemAsync(menuItem);

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
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
    {
        try
        {
            MenuItem? menuItem = await _menuItemServices.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                string errorMessage = $"Menu item not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            await _menuItemServices.DeleteMenuItemAsync(id);
            _imageServices.DeleteImage(menuItem.ImageUrl);

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