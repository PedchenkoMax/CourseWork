namespace Catalog.Api.Services.Abstractions;

public interface IBlobService
{
    public Task UploadFileAsync(string bucketName, string uniqueFileName, IFormFile file);
    public Task DeleteFileAsync(string bucketName, string imageName);
}