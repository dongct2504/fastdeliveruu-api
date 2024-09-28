namespace FastDeliveruu.Application.Dtos.RestaurantDtos;

public class RestaurantHourDto
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    public string? WeekenDay { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
