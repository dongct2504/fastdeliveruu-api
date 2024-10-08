﻿using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;

public class UpdateMenuItemCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    public Guid GenreId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public IFormFile? ImageFile { get; set; }
}
