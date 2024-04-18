using FastDeliveruu.Application.Interfaces;

namespace FastDeliveruu.Api.Services;

public class ImageServices : IImageServices
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public ImageServices(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public async Task<string?> UploadImageAsync(IFormFile? imageFile, string uploadImagePath)
    {
        string webRootPath = _hostEnvironment.WebRootPath;

        if (imageFile != null)
        {
            string uploadPath = Path.Combine(webRootPath, uploadImagePath);
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(imageFile.FileName);

            using (FileStream fs = new FileStream(
                Path.Combine(uploadPath, fileName + extension), FileMode.Create))
            {
                await imageFile.CopyToAsync(fs);
            }

            return fileName + extension;
        }

        return null;
    }

    public async Task DeleteImage(string? imageUrl)
    {
        string webRootPath = _hostEnvironment.WebRootPath;

        if (imageUrl != null)
        {
            string oldImagePath = Path.Combine(webRootPath, imageUrl.TrimStart('\\'));
            if (File.Exists(oldImagePath))
            {
                await Task.Run(() => File.Delete(oldImagePath));
            }
        }
    }
}