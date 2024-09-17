namespace FastDeliveruu.Application.Dtos.MenuVariantDtos;

public class MenuVariantDto
{
    public Guid Id { get; set; }

    public Guid MenuItemId { get; set; }

    public string VarietyName { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }
}
