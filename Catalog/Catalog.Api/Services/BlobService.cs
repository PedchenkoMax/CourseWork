using Catalog.Api.Services.Abstractions;
using Catalog.Infrastructure.BlobStorage.Abstractions;

namespace Catalog.Api.Services;

public class BlobService : IBlobService
{
    private readonly IBlobStorage blobStorage;

    public BlobService(IBlobStorage blobStorage)
    {
        this.blobStorage = blobStorage;
    }

    public async Task UploadFileAsync(string bucketName, IFormFile file)
    {
        var uniqueFileName = GenerateUniqueFileName(file);

        await blobStorage.UploadFileAsync(bucketName, file.OpenReadStream(), uniqueFileName, file.ContentType);
    }

    public async Task DeleteFileAsync(string bucketName, string imageName)
    {
        await blobStorage.DeleteFileAsync(bucketName, imageName);
    }

    private static string GenerateUniqueFileName(IFormFile file)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        var fileExtension = Path.GetExtension(file.FileName);
        var fileGuid = Guid.NewGuid();
        var uniqueFileName = $"{fileNameWithoutExtension}_{fileGuid}{fileExtension}";

        return uniqueFileName;
    }
}