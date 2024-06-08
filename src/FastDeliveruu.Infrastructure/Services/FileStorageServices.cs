using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FastDeliveruu.Infrastructure.Services;

public class FileStorageServices : IFileStorageServices
{
    private readonly Cloudinary _cloudinary;

    public FileStorageServices(IOptions<CloudinarySettings> options)
    {
        Account account = new Account(options.Value.CloudName, options.Value.ApiKey, options.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile imageFile, string uploadImagePath)
    {
        ImageUploadResult imageUploadResult = new ImageUploadResult();

        if (imageFile.Length > 0)
        {
            using (Stream stream = imageFile.OpenReadStream())
            {
                ImageUploadParams uploadParams = new ImageUploadParams
                {
                    Folder = uploadImagePath,
                    File = new FileDescription(imageFile.FileName, stream),
                    Transformation = new Transformation()
                        .Height(500)
                        .Width(500)
                        .Crop("fill")
                        .Gravity("face")
                };
                imageUploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
        }

        return imageUploadResult;
    }

    public async Task<DeletionResult> DeleteImageAsync(string publicId)
    {
        DeletionParams deletionParams = new DeletionParams(publicId);

        DeletionResult deletionResult = await _cloudinary.DestroyAsync(deletionParams);

        return deletionResult;
    }
}
