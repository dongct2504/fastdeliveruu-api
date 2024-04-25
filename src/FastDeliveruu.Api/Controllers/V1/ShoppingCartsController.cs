using System.Security.Claims;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shopping-carts")]
public class ShoppingCartsController : ApiController
{
    private readonly IShoppingCartServices _shoppingCartServices;
    private readonly IMenuItemServices _menuItemServices;
    private readonly ILogger<ShoppingCartsController> _logger;
    private readonly IMapper _mapper;

    public ShoppingCartsController(IShoppingCartServices shoppingCartServices,
        IMenuItemServices menuItemServices,
        ILogger<ShoppingCartsController> logger,
        IMapper mapper)
    {
        _shoppingCartServices = shoppingCartServices;
        _menuItemServices = menuItemServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllShoppingCartsByUserId()
    {
        try
        {
            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            return Ok(_mapper.Map<IEnumerable<ShoppingCartDto>>(
                await _shoppingCartServices.GetAllShoppingCartsByUserIdAsync(userId)));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{menuItemId}", Name = "GetShoppingCartById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetShoppingCartById(long menuItemId)
    {
        try
        {
            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                Unauthorized();
            }

            Result<ShoppingCart> getShoppingCartResult = await _shoppingCartServices.
                GetShoppingCartByUserIdMenuItemIdAsync(userId, menuItemId);
            if (getShoppingCartResult.IsFailed)
            {
                return Problem(getShoppingCartResult.Errors);
            }

            return Ok(_mapper.Map<ShoppingCartDto>(getShoppingCartResult.Value));
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddToCart(
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

            ShoppingCart shoppingCart = _mapper.Map<ShoppingCart>(shoppingCartCreateDto);
            shoppingCart.LocalUserId = userId;
            shoppingCart.CreatedAt = DateTime.Now;
            shoppingCart.UpdatedAt = DateTime.Now;

            Result<ShoppingCart> addToCartResult =
                await _shoppingCartServices.AddToShoppingCartAsync(shoppingCart);
            if (addToCartResult.IsFailed)
            {
                return Problem(addToCartResult.Errors);
            }

            ShoppingCartDto shoppingCartDto = _mapper.Map<ShoppingCartDto>(shoppingCart);

            return CreatedAtRoute(nameof(GetShoppingCartById),
                new { menuItemId = shoppingCart.MenuItemId }, shoppingCartDto);
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
    public async Task<IActionResult> UpdateShoppingCart(long menuItemId,
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

            Result<ShoppingCart> getShoppingCartResult = await
                _shoppingCartServices.GetShoppingCartByUserIdMenuItemIdAsync(userId, menuItemId);
            if (getShoppingCartResult.IsFailed)
            {
                return Problem(getShoppingCartResult.Errors);
            }

            ShoppingCart shoppingCart = getShoppingCartResult.Value;

            _mapper.Map(shoppingCartUpdateDto, shoppingCart);
            shoppingCart.UpdatedAt = DateTime.Now;

            Result updateShoppingCartresult =
                await _shoppingCartServices.UpdateShoppingCartAsync(menuItemId, shoppingCart);
            if (updateShoppingCartresult.IsFailed)
            {
                return Problem(updateShoppingCartresult.Errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{menuItem}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteShoppingCart(long menuItem)
    {
        try
        {
            Guid userId = GetAuthenticationUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            Result deleteShoppingCartresult =
                await _shoppingCartServices.DeleteShoppingCartAsync(userId, menuItem);
            if (deleteShoppingCartresult.IsFailed)
            {
                return Problem(deleteShoppingCartresult.Errors);
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