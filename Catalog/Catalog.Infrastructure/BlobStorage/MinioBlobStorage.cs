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

    public async Task<string?> UploadFileAsync(string bucketName, Stream stream, string uniqueFileName, string contentType)
    {
        try
        {
            var putObjectArgs = new PutObjectArgs()
                                .WithBucket(bucketName)
                                .WithStreamData(stream)
                                .WithObjectSize(stream.Length)
                                .WithContentType(contentType)
                                .WithObject(uniqueFileName);

            var response = await minioClient.PutObjectAsync(putObjectArgs);

            return response.ObjectName;
        }
        catch (MinioException ex)
        {
            // TODO: do i even need try catch here?
            // I see scenarios:
            // 1. blob is down -> error occurs -> error handler -> 500 Internal Error | seems good
            // 2. blob is working, but something is wrong -> ??? same or return null string | seems bad or not?
            // handle exception

            return null;

            // For example, you can log the error or return a custom error message
            // this is also the place where you could potentially throw a custom exception that 
            // your application is designed to handle
            // return null; // or throw new CustomException("Error uploading image", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string bucketName, string fileName)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                                   .WithBucket(bucketName)
                                   .WithObject(fileName);

            await minioClient.RemoveObjectAsync(removeObjectArgs);
            return true;
        }
        catch (MinioException ex)
        {
            // TODO: same as above
            return false;
        }
    }
}