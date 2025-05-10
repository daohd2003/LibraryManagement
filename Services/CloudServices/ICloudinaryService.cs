namespace LibraryManagement.Services.CloudServices
{
    public interface ICloudinaryService
    {
        Task<String> UploadImage(IFormFile file, int userId);
    }
}
