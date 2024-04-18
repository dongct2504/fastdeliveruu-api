using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Interfaces;

public interface IImageServices
{
    Task<string?> UploadImageAsync(IFormFile? imageFile, string uploadImagePath);
    Task DeleteImage(string? ImageUrl);
}