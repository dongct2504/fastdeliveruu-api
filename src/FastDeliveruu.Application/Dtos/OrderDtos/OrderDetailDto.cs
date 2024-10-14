﻿using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderDetailDto
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid MenuItemId { get; set; }

    public Guid? MenuVariantId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string ImageUrl { get; set; } = null!;

    public MenuItemDto MenuItemDto { get; set; } = null!;

    public MenuVariantDto? MenuVariantDto { get; set; }
}
