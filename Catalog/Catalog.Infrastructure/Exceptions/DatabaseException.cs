namespace Catalog.Infrastructure.Exceptions;

public class DatabaseException : Exception
{
    public DatabaseException(Exception innerException) 
        : base("An error occurred with Database.", innerException)
    {
    }
}