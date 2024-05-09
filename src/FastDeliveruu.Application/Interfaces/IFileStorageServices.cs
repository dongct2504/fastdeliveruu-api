using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Interfaces;

public interface IFileStorageServices
{
    Task<string?> UploadImageAsync(IFormFile? imageFile, string uploadImagePath);
    Task DeleteImageAsync(string? ImageUrl);
}