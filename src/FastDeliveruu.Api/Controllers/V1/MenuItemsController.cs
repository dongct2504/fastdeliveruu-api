using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu-items")]
public class MenuItemsController : ControllerBase
{
    private readonly ApiResponse _apiResponse;
    private readonly PaginationResponse _paginationResponse;
    private readonly IMenuItemServices _menuItemServices;
    private readonly ILogger<MenuItemsController> _logger;
    private readonly IMapper _mapper;
    private readonly IImageServices _imageServices;

    public MenuItemsController(IMenuItemServices menuItemServices,
        ILogger<MenuItemsController> logger,
        IMapper mapper,
        IImageServices imageServices)
    {
        _apiResponse = new ApiResponse();
        _paginationResponse = new PaginationResponse();
        _menuItemServices = menuItemServices;
        _logger = logger;
        _mapper = mapper;
        _imageServices = imageServices;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = "Default30",
        VaryByQueryKeys = new[] { "genreId", "restaurantId", "page" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginationResponse>> GetAllMenuItems(int? genreId, int? restaurantId,
        int page = 1)
    {
        try
        {
            IEnumerable<MenuItem> menuItems = await _menuItemServices.GetFilterMenuItemsWithRestaurantGenreAsync(
                genreId, restaurantId, page);

            _paginationResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _paginationResponse.IsSuccess = true;

            _paginationResponse.PageNumber = page;
            _paginationResponse.PageSize = PagingConstants.DefaultPageSize;
            _paginationResponse.TotalRecords = await _menuItemServices.GetTotalMenuItemsAsync();

            _paginationResponse.Result = _mapper.Map<IEnumerable<MenuItemDto>>(menuItems);

            return Ok(_paginationResponse);
        }
        catch (Exception ex)
        {
            _paginationResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _paginationResponse.IsSuccess = false;
            _paginationResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _paginationResponse);
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
            MenuItem? menuItem = await _menuItemServices.GetMenuItemWithRestaurantGenreByIdAsync(id);
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
            _apiResponse.Result = _mapper.Map<MenuItemDto>(menuItem);

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
    public async Task<ActionResult<ApiResponse>> CreateMenuItem(
        [FromBody] MenuItemCreateDto menuItemCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (menuItemCreateDto == null)
            {
                string errorMessage = "Can't create the requested menu item because it is null.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            MenuItem? menuItem = await _menuItemServices.GetMenuItemByNameAsync(
                menuItemCreateDto.Name);
            if (menuItem != null)
            {
                string errorMessage = "Can't create the requested menu item because it already exists.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Conflict;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return Conflict(_apiResponse);
            }

            menuItem = _mapper.Map<MenuItem>(menuItemCreateDto);

            // create and save image
            if (menuItemCreateDto.ImageFile != null)
            {
                string uploadImagePath = @"images\menu-items";
                string? fileNameWithExtension =
                    await _imageServices.UploadImageAsync(menuItemCreateDto.ImageFile, uploadImagePath);
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
    public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id,
        [FromBody] MenuItemUpdateDto menuItemUpdateDto)
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

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            _mapper.Map(menuItemUpdateDto, menuItem);

            if (menuItemUpdateDto.ImageFile != null)
            {
                _imageServices.DeleteImage(menuItem.ImageUrl); // if it has an old one, delete it

                string uploadImagePath = @"images\menu-items";
                string? fileNameWithExtension =
                    await _imageServices.UploadImageAsync(menuItemUpdateDto.ImageFile, uploadImagePath);
                menuItem.ImageUrl = @"\images\menu-items\" + fileNameWithExtension;
            }

            menuItem.UpdatedAt = DateTime.Now;

            await _menuItemServices.UpdateMenuItemAsync(menuItem);

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
    public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
    {
        try
        {
            MenuItem? menuItem = await _menuItemServices.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                string errorMessage = $"Menu item not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            await _menuItemServices.DeleteMenuItemAsync(id);
            _imageServices.DeleteImage(menuItem.ImageUrl);

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