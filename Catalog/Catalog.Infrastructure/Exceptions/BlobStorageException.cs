namespace Catalog.Infrastructure.Exceptions;

public class BlobStorageException : Exception
{
    public BlobStorageException(Exception innerException) 
        : base("An error occurred with BlobStorage.", innerException)
    {
    }
}