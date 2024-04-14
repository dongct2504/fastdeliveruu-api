using AutoMapper;
using FastDeliveruu.Application.Common.Roles;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu-items")]
public class MenuItemsController : ApiController
{
    private readonly PaginationResponse<MenuItemDto> _paginationResponse;
    private readonly IMenuItemServices _menuItemServices;
    private readonly ILogger<MenuItemsController> _logger;
    private readonly IMapper _mapper;
    private readonly IImageServices _imageServices;

    public MenuItemsController(IMenuItemServices menuItemServices,
        ILogger<MenuItemsController> logger,
        IMapper mapper,
        IImageServices imageServices)
    {
        _paginationResponse = new PaginationResponse<MenuItemDto>();
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
    public async Task<IActionResult> GetAllMenuItems(int? genreId, Guid? restaurantId, int page = 1)
    {
        try
        {
            IEnumerable<MenuItem> menuItems = await _menuItemServices.GetFilterMenuItemsWithRestaurantGenreAsync(
                genreId, restaurantId, page);

            _paginationResponse.PageNumber = page;
            _paginationResponse.PageSize = PagingConstants.DefaultPageSize;
            _paginationResponse.TotalRecords = await _menuItemServices.GetTotalMenuItemsAsync();

            _paginationResponse.Values = _mapper.Map<IEnumerable<MenuItemDto>>(menuItems);

            return Ok(_paginationResponse);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id}", Name = "GetMenuItemById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMenuItemById(Guid id)
    {
        try
        {
            Result<MenuItem> getMenuItemResult =
                await _menuItemServices.GetMenuItemWithRestaurantGenreByIdAsync(id);
            if (getMenuItemResult.IsFailed)
            {
                return Problem(getMenuItemResult.Errors);
            }

            return Ok(_mapper.Map<MenuItemDetailDto>(getMenuItemResult.Value));
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
    public async Task<IActionResult> CreateMenuItem([FromForm] MenuItemCreateDto menuItemCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MenuItem menuItem = _mapper.Map<MenuItem>(menuItemCreateDto);

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

            Result<Guid> createMenuItemResult = await _menuItemServices.CreateMenuItemAsync(menuItem);
            if (createMenuItemResult.IsFailed)
            {
                _imageServices.DeleteImage(menuItem.ImageUrl);

                return Problem(createMenuItemResult.Errors);
            }

            menuItem.MenuItemId = createMenuItemResult.Value;

            MenuItemDto menuItemDto = _mapper.Map<MenuItemDto>(menuItem);

            return CreatedAtRoute(nameof(GetMenuItemById), new { Id = menuItemDto.MenuItemId }, menuItemDto);
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
    public async Task<IActionResult> UpdateMenuItem(Guid id, [FromForm] MenuItemUpdateDto menuItemUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<MenuItem> oldMenuItemResult = await _menuItemServices.GetMenuItemByIdAsync(id);
            if (oldMenuItemResult.IsFailed)
            {
                return Problem(oldMenuItemResult.Errors);
            }

            MenuItem menuItem = oldMenuItemResult.Value;

            _mapper.Map(menuItemUpdateDto, menuItem);

            if (menuItemUpdateDto.ImageFile != null)
            {
                // if it has an old one, delete it
                _imageServices.DeleteImage(oldMenuItemResult.Value.ImageUrl);

                string uploadImagePath = @"images\menu-items";
                string? fileNameWithExtension =
                    await _imageServices.UploadImageAsync(menuItemUpdateDto.ImageFile, uploadImagePath);
                menuItem.ImageUrl = @"\images\menu-items\" + fileNameWithExtension;
            }
            menuItem.UpdatedAt = DateTime.Now;

            Result updateMenuItemResult = await _menuItemServices.UpdateMenuItemAsync(id, menuItem);
            if (updateMenuItemResult.IsFailed)
            {
                return Problem(updateMenuItemResult.Errors);
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
    public async Task<IActionResult> DeleteMenuItem(Guid id)
    {
        try
        {
            Result<MenuItem> menuItemDeleteResult = await _menuItemServices.GetMenuItemByIdAsync(id);
            if (menuItemDeleteResult.IsFailed)
            {
                return Problem(menuItemDeleteResult.Errors);
            }

            await _menuItemServices.DeleteMenuItemAsync(id);

            _imageServices.DeleteImage(menuItemDeleteResult.Value.ImageUrl);

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}