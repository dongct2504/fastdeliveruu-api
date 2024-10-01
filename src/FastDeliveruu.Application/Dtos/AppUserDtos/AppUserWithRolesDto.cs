namespace FastDeliveruu.Application.Dtos.AppUserDtos;

public class AppUserWithRolesDto
{
    public string Id { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string Roles { get; set; } = null!;
}
