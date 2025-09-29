namespace FastDeliveruu.Application.Dtos.AddressDtos;

public class CityDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DistrictDto> DistrictDtos { get; set; } = new List<DistrictDto>();
}
