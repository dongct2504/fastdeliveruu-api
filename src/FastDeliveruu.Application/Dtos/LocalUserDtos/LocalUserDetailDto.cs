using FastDeliveruu.Application.Dtos.OrderDtos;

namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class LocalUserDetailDto
{
    public Guid LocalUserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public string Role { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Address { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public IEnumerable<OrderDto> OrderDtos { get; set; } = null!;
}
