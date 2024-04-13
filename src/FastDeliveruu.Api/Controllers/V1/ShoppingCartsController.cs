using System.Security.Claims;
using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shopping-carts")]
public class ShoppingCartsController : ControllerBase
{
    private readonly ApiResponse _apiResponse;
    private readonly PaginationResponse _paginationResponse;
    private readonly IShoppingCartServices _shoppingCartServices;
    private readonly IMenuItemServices _menuItemServices;
    private readonly ILogger<ShoppingCartsController> _logger;
    private readonly IMapper _mapper;

    public ShoppingCartsController(IShoppingCartServices shoppingCartServices,
        IMenuItemServices menuItemServices,
        ILogger<ShoppingCartsController> logger,
        IMapper mapper)
    {
        _apiResponse = new ApiResponse();
        _paginationResponse = new PaginationResponse();
        _shoppingCartServices = shoppingCartServices;
        _menuItemServices = menuItemServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginationResponse>> GetAllShoppingCartsByUserId(int page = 1)
    {
        try
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            Claim? claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                string errorMessage = "Can't get the requested shopping carts because " +
                    "the user is not authenticated.";

                _paginationResponse.HttpStatusCode = System.Net.HttpStatusCode.Unauthorized;
                _paginationResponse.IsSuccess = false;
                _paginationResponse.ErrorMessages = new List<string> { errorMessage };

                return Unauthorized(_paginationResponse);
            }

            int userId = int.Parse(claim.Value);

            _paginationResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _paginationResponse.IsSuccess = true;

            _paginationResponse.PageNumber = page;
            _paginationResponse.PageSize = PagingConstants.ShoppingCartPageSize;
            _paginationResponse.TotalRecords = await _shoppingCartServices.
                GetTotalShoppingCartsByUserIdAsync(userId);

            _paginationResponse.Result = _mapper.Map<IEnumerable<ShoppingCartDto>>(
                await _shoppingCartServices.GetAllShoppingCartsByUserIdAsync(userId, page));

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

    [HttpGet("{id:int}", Name = "GetShoppingCartById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetShoppingCartById(int id)
    {
        try
        {
            ShoppingCart? shoppingCart = await _shoppingCartServices.GetShoppingCartByIdAsync(id);
            if (shoppingCart == null)
            {
                string errorMessage = $"Shopping Cart not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<ShoppingCartDto>(shoppingCart);

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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateShoppingCart(
        [FromBody] ShoppingCartCreateDto shoppingCartCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (shoppingCartCreateDto == null)
            {
                string errorMessage = "Can't create the requested shopping cart because it is null.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return BadRequest(_apiResponse);
            }

            MenuItem? menuItem = await _menuItemServices.GetMenuItemByIdAsync(shoppingCartCreateDto.MenuItemId);
            if (menuItem == null)
            {
                string errorMessage = "Can't create the requested shopping cart because menu item is null.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            Claim? claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                string errorMessage = "Can't create the requested shopping carts because " +
                    "the user is not authenticated.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Unauthorized;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return Unauthorized(_apiResponse);
            }

            int userId = int.Parse(claim.Value);

            // get a unique shopping cart through LocalUserId and MenuItemId
            ShoppingCart? shoppingCart = await _shoppingCartServices.GetShoppingCartByUserIdMenuItemIdAsync(
                userId, shoppingCartCreateDto.MenuItemId);
            if (shoppingCart != null)
            {
                string errorMessage = "Can't create the requested shopping cart because it already exists.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Conflict;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return Conflict(_apiResponse);
            }

            shoppingCart = _mapper.Map<ShoppingCart>(shoppingCartCreateDto);
            shoppingCart.LocalUserId = userId;
            shoppingCart.CreatedAt = DateTime.Now;
            shoppingCart.UpdatedAt = DateTime.Now;

            int createdShoppingCartId = await _shoppingCartServices.CreateShoppingCartAsync(shoppingCart);
            shoppingCart.ShoppingCartId = createdShoppingCartId;

            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = _mapper.Map<ShoppingCartDto>(shoppingCart);

            return CreatedAtRoute(nameof(GetShoppingCartById), new { Id = createdShoppingCartId }, _apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(500, _apiResponse);
        }
    }

    [HttpPut("{menuItemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> UpdateShoppingCart(int menuItemId,
        [FromBody] ShoppingCartUpdateDto shoppingCartUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            Claim? claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                string errorMessage = "Can't update the requested shopping carts because " +
                    "the user is not authenticated.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.Unauthorized;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return Unauthorized(_apiResponse);
            }

            int userId = int.Parse(claim.Value);

            ShoppingCart? shoppingCart = await _shoppingCartServices.
                GetShoppingCartByUserIdMenuItemIdAsync(userId, menuItemId);
            if (shoppingCart == null)
            {
                string errorMessage = $"shopping cart not found";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            shoppingCart.Quantity += shoppingCartUpdateDto.Quantity;
            shoppingCart.UpdatedAt = DateTime.Now;

            await _shoppingCartServices.UpdateShoppingCartAsync(shoppingCart);

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> DeleteShoppingCart(int id)
    {
        try
        {
            ShoppingCart? shoppingCart = await _shoppingCartServices.GetShoppingCartByIdAsync(id);
            if (shoppingCart == null)
            {
                string errorMessage = $"Genre not found. The requested id: '{id}' does not exist.";

                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { errorMessage };

                return NotFound(_apiResponse);
            }

            await _shoppingCartServices.DeleteShoppingCartAsync(id);

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