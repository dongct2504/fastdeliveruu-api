using FastDeliveruu.Application.Interfaces;

namespace FastDeliveruu.Api.Services;

public class FileStorageServices : IFileStorageServices
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public FileStorageServices(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public async Task<string?> UploadImageAsync(IFormFile? imageFile, string uploadImagePath)
    {
        if (imageFile == null)
        {
            return null;
        }

        string webRootPath = _hostEnvironment.WebRootPath;

        string uploadPath = $"{webRootPath}{uploadImagePath}";
        string fileName = Guid.NewGuid().ToString();
        string extension = Path.GetExtension(imageFile.FileName);

        using (FileStream fs = new FileStream($"{uploadPath}{fileName}{extension}", FileMode.Create))
        {
            await imageFile.CopyToAsync(fs);
        }

        return fileName + extension;
    }

    public async Task DeleteImageAsync(string? imageUrl)
    {
        if (imageUrl == null)
        {
            return;
        }

        string webRootPath = _hostEnvironment.WebRootPath;

        string oldImagePath = Path.Combine(webRootPath, imageUrl.TrimStart('\\'));
        if (File.Exists(oldImagePath))
        {
            await Task.Run(() => File.Delete(oldImagePath));
        }
    }
}