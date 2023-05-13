using System.Data;
using Npgsql;

namespace Catalog.Infrastructure.Database;

public class DbContext
{
    private readonly string connectionString;

    public IDbConnection Connection => new NpgsqlConnection(connectionString);

    public DbContext(string connectionString)
    {
        this.connectionString = connectionString;
    }
}