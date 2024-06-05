using System.Security.Claims;

namespace FastDeliveruu.Domain.Extensions;

public static class LocalUserExtensions
{
    public static Guid GetCurrentUserId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
    }
}
