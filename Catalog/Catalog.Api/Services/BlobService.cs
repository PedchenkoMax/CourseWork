using Catalog.Api.Services.Abstractions;
using Catalog.Infrastructure.BlobStorage.Abstractions;

namespace Catalog.Api.Services;

public class BlobService : IBlobService
{
    private readonly ILogger<BlobService> logger;
    private readonly IBlobStorage blobStorage;

    public BlobService(ILogger<BlobService> logger, IBlobStorage blobStorage)
    {
        this.logger = logger;
        this.blobStorage = blobStorage;
    }

    public async Task UploadFileAsync(string bucketName, string uniqueFileName, IFormFile file)
    {
        logger.LogInformation("Uploading file {UniqueFileName} to bucket {BucketName}...", uniqueFileName, bucketName);
        await blobStorage.UploadFileAsync(bucketName, file.OpenReadStream(), uniqueFileName, file.ContentType);
        logger.LogInformation("File {UniqueFileName} uploaded successfully to bucket {BucketName}", uniqueFileName, bucketName);
    }

    public async Task DeleteFileAsync(string bucketName, string imageName)
    {
        logger.LogInformation("Deleting file {ImageName} from bucket {BucketName}...", imageName, bucketName);
        await blobStorage.DeleteFileAsync(bucketName, imageName);
        logger.LogInformation("File {ImageName} deleted successfully from bucket {BucketName}", imageName, bucketName);
    }

    public static string GenerateUniqueFileName(IFormFile file)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        var fileExtension = Path.GetExtension(file.FileName);
        var fileGuid = Guid.NewGuid();
        var uniqueFileName = $"{fileNameWithoutExtension}_{fileGuid}{fileExtension}";

        return uniqueFileName;
    }
}