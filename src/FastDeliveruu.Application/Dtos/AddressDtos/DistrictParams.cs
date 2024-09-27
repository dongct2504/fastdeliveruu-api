namespace FastDeliveruu.Application.Dtos.AddressDtos;

public class DistrictParams : DefaultParams
{
    public int CityId { get; set; }

    public override string? ToString()
    {
        return $"{CityId}-{Sort}-{Search}-{PageNumber}-{PageSize}";
    }
}
