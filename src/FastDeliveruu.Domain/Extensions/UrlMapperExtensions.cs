namespace FastDeliveruu.Domain.Extensions;

public static class UrlMapperExtensions
{
    public static string? MapImageUrl(this string? imageUrl, string baseUrl)
    {
        return string.IsNullOrEmpty(imageUrl) ? null : baseUrl + imageUrl;
    }
}
