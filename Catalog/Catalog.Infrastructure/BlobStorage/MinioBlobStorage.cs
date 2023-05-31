using Catalog.Infrastructure.BlobStorage.Abstractions;
using Minio;
using Minio.Exceptions;

namespace Catalog.Infrastructure.BlobStorage;

public class MinioBlobStorage : IBlobStorage
{
    private readonly MinioClient minioClient;

    public MinioBlobStorage(MinioClient minioClient)
    {
        this.minioClient = minioClient;
    }

    public async Task UploadFileAsync(string bucketName, Stream stream, string uniqueFileName, string contentType)
    {
        try
        {
            var putObjectArgs = new PutObjectArgs()
                                .WithBucket(bucketName)
                                .WithStreamData(stream)
                                .WithObjectSize(stream.Length)
                                .WithContentType(contentType)
                                .WithObject(uniqueFileName);

            await minioClient.PutObjectAsync(putObjectArgs);
        }
        catch (MinioException ex)
        {
            // log here
            throw;
        }
    }

    public async Task DeleteFileAsync(string bucketName, string fileName)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                                   .WithBucket(bucketName)
                                   .WithObject(fileName);

            await minioClient.RemoveObjectAsync(removeObjectArgs);
        }
        catch (MinioException ex)
        {
            // log here
            throw;
        }
    }
}