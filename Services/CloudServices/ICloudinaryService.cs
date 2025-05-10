namespace LibraryManagement.Services.CloudServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadImage(IFormFile file, int userId, string projectName, string folderType);
    }
}
