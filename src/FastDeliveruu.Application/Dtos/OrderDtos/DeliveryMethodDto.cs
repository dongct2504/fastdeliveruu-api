namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class DeliveryMethodDto
{
    public int Id { get; set; }

    public string ShortName { get; set; } = null!;

    public string? EstimatedDeliveryTime { get; set; }

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }
}
