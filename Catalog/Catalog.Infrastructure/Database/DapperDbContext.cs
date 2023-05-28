using System.Data;
using Npgsql;

namespace Catalog.Infrastructure.Database;

public class DapperDbContext
{
    private readonly string connectionString;

    public IDbConnection Connection => new NpgsqlConnection(connectionString);

    public DapperDbContext(string connectionString)
    {
        this.connectionString = connectionString;
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}