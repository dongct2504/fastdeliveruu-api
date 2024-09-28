﻿namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuVariantDto
{
    public Guid Id { get; set; }

    public Guid MenuItemId { get; set; }

    public string VarietyName { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount => Price * DiscountPercent;

    public decimal DiscountPrice => Price - DiscountAmount;

    public string ImageUrl { get; set; } = null!;
}