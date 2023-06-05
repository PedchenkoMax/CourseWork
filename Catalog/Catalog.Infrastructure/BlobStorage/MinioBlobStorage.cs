using Catalog.Infrastructure.BlobStorage.Abstractions;
using Catalog.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.Exceptions;

namespace Catalog.Infrastructure.BlobStorage;

public class MinioBlobStorage : IBlobStorage
{
    private readonly ILogger<MinioBlobStorage> logger;
    private readonly MinioClient minioClient;

    public MinioBlobStorage(ILogger<MinioBlobStorage> logger, MinioClient minioClient)
    {
        this.logger = logger;
        this.minioClient = minioClient;
    }

    public async Task UploadFileAsync(string bucketName, Stream stream, string uniqueFileName, string contentType)
    {
        try
        {
            logger.LogInformation("Uploading file {UniqueFileName} to Minio bucket {BucketName}...", uniqueFileName, bucketName);

            var putObjectArgs = new PutObjectArgs()
                                .WithBucket(bucketName)
                                .WithStreamData(stream)
                                .WithObjectSize(stream.Length)
                                .WithContentType(contentType)
                                .WithObject(uniqueFileName);

            await minioClient.PutObjectAsync(putObjectArgs);
            logger.LogInformation("File {UniqueFileName} uploaded successfully to Minio bucket {BucketName}", uniqueFileName, bucketName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while uploading file {UniqueFileName} to Minio bucket {BucketName}", uniqueFileName, bucketName);
            throw new BlobStorageException(ex);
        }
    }

    public async Task DeleteFileAsync(string bucketName, string fileName)
    {
        try
        {
            logger.LogInformation("Deleting file {FileName} from Minio bucket {BucketName}...", fileName, bucketName);
            var removeObjectArgs = new RemoveObjectArgs()
                                   .WithBucket(bucketName)
                                   .WithObject(fileName);

            await minioClient.RemoveObjectAsync(removeObjectArgs);
            logger.LogInformation("File {FileName} deleted successfully from Minio bucket {BucketName}", fileName, bucketName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting file {FileName} from Minio bucket {BucketName}", fileName, bucketName);
            throw new BlobStorageException(ex);
        }
    }
}