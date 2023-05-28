using Catalog.Infrastructure.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Catalog.IntegrationTests.TestUtils;

public class DatabaseFixture : IAsyncLifetime
{
    public string ConnectionString { get; private set; } = null!;
    private readonly PostgreSqlContainer container;

    public DatabaseFixture()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        container = new PostgreSqlBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        ConnectionString = container.GetConnectionString();

        var migrationRunner = new MigrationRunner();
        migrationRunner.RunMigrations(ConnectionString);
    }

    public Task DisposeAsync()
    {
        return container.DisposeAsync().AsTask();
    }
}