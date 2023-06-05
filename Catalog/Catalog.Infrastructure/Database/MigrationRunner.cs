using Catalog.Infrastructure.Database.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Database;

public class MigrationRunner
{
    private readonly ILogger<MigrationRunner> logger;
    private readonly string connectionString;

    public MigrationRunner(ILogger<MigrationRunner> logger, string connectionString)
    {
        this.logger = logger;
        this.connectionString = connectionString;
    }

    public void RunMigrations()
    {
        var serviceProvider = CreateServices();

        using var scope = serviceProvider.CreateScope();

        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        try
        {
            runner.MigrateUp();
            logger.LogInformation("The database is up-to-date");
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while migrating the database: {Exception}", ex);
        }
    }

    private IServiceProvider CreateServices()
    {
        return new ServiceCollection()
               .AddFluentMigratorCore()
               .ConfigureRunner(runnerBuilder
                   => runnerBuilder
                      .AddPostgres()
                      .WithGlobalConnectionString(connectionString)
                      .ScanIn(typeof(Initial).Assembly).For.Migrations())
               .AddLogging(lb => lb.AddFluentMigratorConsole())
               .BuildServiceProvider(false);
    }
}