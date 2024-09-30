namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemInventoryDto
{
    public Guid Id { get; set; }

    public Guid MenuItemId { get; set; }

    public int QuantityAvailable { get; set; }

    public int QuantityReserved { get; set; }
}
