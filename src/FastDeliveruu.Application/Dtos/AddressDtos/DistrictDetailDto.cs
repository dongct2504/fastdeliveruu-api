namespace FastDeliveruu.Application.Dtos.AddressDtos;

public class DistrictDetailDto
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<WardDto> WardDtos { get; set; } = new List<WardDto>();
}
