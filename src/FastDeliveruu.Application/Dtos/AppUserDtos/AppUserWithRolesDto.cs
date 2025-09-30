namespace FastDeliveruu.Application.Dtos.AppUserDtos;

public class AppUserWithRolesDto
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public bool IsLocked { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}
