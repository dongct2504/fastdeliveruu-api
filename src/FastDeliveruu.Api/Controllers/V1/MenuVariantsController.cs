﻿using Asp.Versioning;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.MenuItems.Commands.DeleteMenuItem;
using FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;
using FastDeliveruu.Application.MenuVariants.Commands.DeleteMenuVariant;
using FastDeliveruu.Application.MenuVariants.Commands.UpdateMenuVariant;
using FastDeliveruu.Application.MenuVariants.Queries.GetByMenuItem;
using FastDeliveruu.Application.MenuVariants.Queries.GetMenuVariantById;
using FastDeliveruu.Domain.Extensions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu-variants")]
public class MenuVariantsController : ApiController
{
    private readonly IMediator _mediator;

    public MenuVariantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-by-menuitem/{menuItemId:guid}")]
    [ProducesResponseType(typeof(List<MenuVariantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMenuItem(Guid menuItemId)
    {
        GetByMenuItemQuery query = new GetByMenuItemQuery(menuItemId);
        List<MenuVariantDto> menuVariantDtos = await _mediator.Send(query);
        return Ok(menuVariantDtos);
    }

    [HttpGet("{id:guid}", Name = "GetMenuVariantById")]
    [ProducesResponseType(typeof(MenuVariantDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMenuVariantById(Guid id)
    {
        GetMenuVariantByIdQuery query = new GetMenuVariantByIdQuery(id);
        Result<MenuVariantDto> result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(typeof(MenuVariantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMenuVariant([FromForm] CreateMenuVariantCommand command)
    {
        Result<MenuVariantDto> result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return CreatedAtRoute(
            nameof(GetMenuVariantById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMenuVariant(Guid id, [FromForm] UpdateMenuVariantCommand command)
    {
        if (id != command.Id)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Id not match.");
        }

        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyConstants.ManageResources)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMenuVariant(Guid id)
    {
        DeleteMenuVariantCommand command = new DeleteMenuVariantCommand(id, User.GetCurrentUserId());
        Result result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Problem(result.Errors);
        }

        return NoContent();
    }
}
