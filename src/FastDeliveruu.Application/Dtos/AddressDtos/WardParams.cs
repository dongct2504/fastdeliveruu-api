namespace FastDeliveruu.Application.Dtos.AddressDtos;

public class WardParams : DefaultParams
{
    public int DistrictId { get; set; }

    public override string? ToString()
    {
        return $"{DistrictId}-{Sort}-{Search}-{PageNumber}-{PageSize}";
    }
}
