namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class DeliveryMethodDto
{
    public Guid DeliveryMethodId { get; set; }

    public string ShortName { get; set; } = null!;

    public string? DeliveryTime { get; set; }

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }
}
