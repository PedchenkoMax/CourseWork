namespace Catalog.Infrastructure.Database.Exceptions;

public class DatabaseException : Exception
{
    public DatabaseException(Exception innerException) 
        : base("An error occurred while querying the database", innerException)
    {
    }
}