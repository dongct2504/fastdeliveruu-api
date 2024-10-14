namespace FastDeliveruu.Application.Dtos.AppUserDtos;

public class AppUserDto
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Address { get; set; }

    public int? CityId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
