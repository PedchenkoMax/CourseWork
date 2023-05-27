namespace Catalog.Api.Services.Abstractions;

public interface IBlobService
{
    public Task<string?> UploadFileAsync(string bucketName, IFormFile file);
    public Task<bool> DeleteFileAsync(string bucketName, string imageName);
}