using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Interfaces;

public interface IFileStorageServices
{
    Task<ImageUploadResult> UploadImageAsync(IFormFile imageFile, string uploadImagePath);
    Task<DeletionResult> DeleteImageAsync(string publicId);
}