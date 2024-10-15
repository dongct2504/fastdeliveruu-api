namespace FastDeliveruu.Application.Dtos.WishListDtos;

public class WishListParams : DefaultParams
{
    public Guid UserId { get; set; }

    public override string? ToString()
    {
        return $"{UserId}-{Sort}-{Search}-{PageNumber}-{PageSize}";
    }
}
