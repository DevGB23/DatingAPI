using CloudinaryDotNet.Actions;

namespace Dating_WebAPI.Helpers;
public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
