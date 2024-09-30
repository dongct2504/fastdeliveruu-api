namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuVariantInventoryDto
{
    public Guid Id { get; set; }

    public Guid MenuVariantId { get; set; }

    public int QuantityAvailable { get; set; }

    public int QuantityReserved { get; set; }
}
