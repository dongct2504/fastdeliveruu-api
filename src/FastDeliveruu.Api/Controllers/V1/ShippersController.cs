using AutoMapper;
using FastDeliveruu.Application.Common.Roles;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shippers")]
public class ShippersController : ApiController
{
    private readonly IShipperService _shipperService;
    private readonly ILogger<ShippersController> _logger;
    private readonly IMapper _mapper;

    public ShippersController(IShipperService shipperService,
        IMapper mapper,
        ILogger<ShippersController> logger)
    {
        _shipperService = shipperService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.RoleAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllShippers()
    {
        try
        {
            return Ok(_mapper.Map<IEnumerable<ShipperDto>>(await _shipperService.GetAllShippersAsync()));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpGet("{id}", Name = "GetShipperById")]
    [Authorize(Roles = RoleConstants.RoleAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetShipperById(Guid id)
    {
        try
        {
            Result<Shipper> getShipperResult = await _shipperService.GetShipperWithOrdersByIdAsync(id);
            if (getShipperResult.IsFailed)
            {
                return Problem(getShipperResult.Errors);
            }

            return Ok(_mapper.Map<ShipperDto>(getShipperResult.Value));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateShipper(Guid id, [FromForm] ShipperUpdateDto shipperUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<Shipper> getShipperResult = await _shipperService.GetShipperByIdAsync(id);
            if (getShipperResult.IsFailed)
            {
                return Problem(getShipperResult.Errors);
            }

            Shipper shipper = getShipperResult.Value;

            _mapper.Map(shipperUpdateDto, shipper);

            shipper.UpdatedAt = DateTime.Now;

            Result updateShipperResult = await _shipperService.DeleteShipperAsync(id);
            if (updateShipperResult.IsFailed)
            {
                return Problem(getShipperResult.Errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.RoleAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteShipper(Guid id)
    {
        try
        {
            Result<Shipper> deleteShipperResult = await _shipperService.DeleteShipperAsync(id);
            if (deleteShipperResult.IsFailed)
            {
                return Problem(deleteShipperResult.Errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.ToString());
        }
    }
}