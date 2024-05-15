using FastDeliveruu.Application.Dtos.OrderDtos;

namespace FastDeliveruu.Application.Dtos.ShipperDtos;

public class ShipperDetailDto
{
    public Guid ShipperId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Cccd { get; set; } = null!;

    public string DriverLicense { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public string? ImageUrl { get; set; }

    public string? VehicleType { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public IEnumerable<OrderDto> OrderDtos { get; set; } = null!;
}
