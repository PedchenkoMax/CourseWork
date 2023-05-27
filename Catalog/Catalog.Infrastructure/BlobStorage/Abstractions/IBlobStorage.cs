namespace Catalog.Infrastructure.BlobStorage.Abstractions;

public interface IBlobStorage
{
    Task<string?> UploadFileAsync(string bucketName, Stream stream, string uniqueFileName, string contentType);
    Task<bool> DeleteFileAsync(string bucketName, string fileName);
}