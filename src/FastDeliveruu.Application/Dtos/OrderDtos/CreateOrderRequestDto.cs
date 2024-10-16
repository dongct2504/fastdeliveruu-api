namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class CreateOrderRequestDto
{
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string Reference { get; set; } = string.Empty;   
}
