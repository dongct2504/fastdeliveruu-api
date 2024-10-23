namespace FastDeliveruu.Application.Dtos.AppUserDtos;

public class AppUserDetailDto
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public bool PhoneNumberConfirmed { get; set; }

    public string? ImageUrl { get; set; }

    public string HouseNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
