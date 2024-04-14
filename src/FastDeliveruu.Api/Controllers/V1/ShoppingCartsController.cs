using System.Security.Claims;
using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shopping-carts")]
public class ShoppingCartsController : ControllerBase
{
    private readonly PaginationResponse<ShoppingCartDto> _paginationResponse;
    private readonly IShoppingCartServices _shoppingCartServices;
    private readonly IMenuItemServices _menuItemServices;
    private readonly ILogger<ShoppingCartsController> _logger;
    private readonly IMapper _mapper;

    public ShoppingCartsController(IShoppingCartServices shoppingCartServices,
        IMenuItemServices menuItemServices,
        ILogger<ShoppingCartsController> logger,
        IMapper mapper)
    {
        _paginationResponse = new PaginationResponse<ShoppingCartDto>();
        _shoppingCartServices = shoppingCartServices;
        _menuItemServices = menuItemServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllShoppingCartsByUserId(int page = 1)
    {
        try
        {
            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            _paginationResponse.PageNumber = page;
            _paginationResponse.PageSize = PagingConstants.ShoppingCartPageSize;
            _paginationResponse.TotalRecords = await _shoppingCartServices.
                GetTotalShoppingCartsByUserIdAsync(userId);

            _paginationResponse.Values = _mapper.Map<IEnumerable<ShoppingCartDto>>(
                await _shoppingCartServices.GetAllShoppingCartsByUserIdAsync(userId, page));

            return Ok(_paginationResponse);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id}", Name = "GetShoppingCartById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetShoppingCartById(int id)
    {
        try
        {
            Result<ShoppingCart> shoppingCartResult = await _shoppingCartServices.GetShoppingCartByIdAsync(id);
            if (shoppingCartResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: shoppingCartResult.Errors[0].Message);
            }

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCartResult.Value));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateShoppingCart(
        [FromBody] ShoppingCartCreateDto shoppingCartCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            Result<MenuItem> menuItemResult =
                await _menuItemServices.GetMenuItemByIdAsync(shoppingCartCreateDto.MenuItemId);
            if (menuItemResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: menuItemResult.Errors[0].Message);
            }

            ShoppingCart shoppingCart = _mapper.Map<ShoppingCart>(shoppingCartCreateDto);
            shoppingCart.LocalUserId = userId;
            shoppingCart.CreatedAt = DateTime.Now;
            shoppingCart.UpdatedAt = DateTime.Now;

            Result<int> shoppingCartResult =
                await _shoppingCartServices.CreateShoppingCartAsync(shoppingCart);
            if (shoppingCartResult.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status409Conflict,
                    detail: shoppingCartResult.Errors[0].Message);
            }

            shoppingCart.ShoppingCartId = shoppingCartResult.Value;

            ShoppingCartDto shoppingCartDto = _mapper.Map<ShoppingCartDto>(shoppingCart);

            return CreatedAtRoute(nameof(GetShoppingCartById),
                new { Id = shoppingCartDto.ShoppingCartId }, shoppingCartDto);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPut("{menuItemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateShoppingCart(Guid menuItemId,
        [FromBody] ShoppingCartUpdateDto shoppingCartUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            ShoppingCart shoppingCart = _mapper.Map<ShoppingCart>(shoppingCartUpdateDto);
            shoppingCart.LocalUserId = userId;
            shoppingCart.Quantity += shoppingCartUpdateDto.Quantity;
            shoppingCart.UpdatedAt = DateTime.Now;

            Result result = await _shoppingCartServices.UpdateShoppingCartAsync(menuItemId, shoppingCart);
            if (result.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: result.Errors[0].Message);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteShoppingCart(int id)
    {
        try
        {
            Result result = await _shoppingCartServices.DeleteShoppingCartAsync(id);
            if (result.IsFailed)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: result.Errors[0].Message);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [NonAction]
    private Guid GetAuthenticationUserId()
    {
        ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
        Claim? claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return Guid.Empty;
        }

        return Guid.Parse(claim.Value);
    }
}