namespace FastDeliveruu.Application.Dtos.ShipperDtos;

public class AuthenticationShipperResult
{
    public ShipperDto ShipperDto { get; set; } = null!;

    public string Token { get; set; } = null!;
}