namespace Catalog.Infrastructure.Exceptions;

public class CacheException : Exception
{
    public CacheException(Exception innerException) 
        : base("An error occurred with Cache.", innerException)
    {
    }
}