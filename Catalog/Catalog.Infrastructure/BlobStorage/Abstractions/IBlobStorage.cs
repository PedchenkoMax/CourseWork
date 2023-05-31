namespace Catalog.Infrastructure.BlobStorage.Abstractions;

public interface IBlobStorage
{
    Task UploadFileAsync(string bucketName, Stream stream, string uniqueFileName, string contentType);
    Task DeleteFileAsync(string bucketName, string fileName);
}