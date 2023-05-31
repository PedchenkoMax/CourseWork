namespace Catalog.Api.Services.Abstractions;

public interface IBlobService
{
    public Task UploadFileAsync(string bucketName, IFormFile file);
    public Task DeleteFileAsync(string bucketName, string imageName);
}