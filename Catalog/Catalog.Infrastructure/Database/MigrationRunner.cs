using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Database;

// TODO: It works, but i don't like how it looks
// like, in future i would be using logger 
// and here it wouldn't use needed one
// https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/
public class MigrationRunner
{
    public void RunMigrations(string connectionString, bool isMigrationUp = true)
    {
        var serviceProvider = CreateServices(connectionString);

        using var scope = serviceProvider.CreateScope();

        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        if (isMigrationUp)
            runner.MigrateUp();
        else
            runner.MigrateDown(0);
    }

    private IServiceProvider CreateServices(string connectionString)
    {
        return new ServiceCollection()
               .AddFluentMigratorCore()
               .ConfigureRunner(runnerBuilder
                   => runnerBuilder
                      .AddPostgres()
                      .WithGlobalConnectionString(connectionString)
                      .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations())
               .AddLogging(lb => lb.AddFluentMigratorConsole())
               .BuildServiceProvider(false);
    }
}