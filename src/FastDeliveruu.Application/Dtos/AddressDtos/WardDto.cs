namespace FastDeliveruu.Application.Dtos.AddressDtos;

public class WardDto
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
