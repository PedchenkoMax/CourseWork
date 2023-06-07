using System.Reflection;
using Catalog.Api.Mappers;
using Catalog.Api.Mappers.Abstractions;
using Catalog.Api.Middlewares;
using Catalog.Api.Services;
using Catalog.Api.Services.Abstractions;
using Catalog.Infrastructure.BlobStorage;
using Catalog.Infrastructure.BlobStorage.Abstractions;
using Catalog.Infrastructure.Cache.Repositories;
using Catalog.Infrastructure.Cache.Services;
using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Minio;
using Minio.AspNetCore;
using Minio.AspNetCore.HealthChecks;
using RabbitMQ.Client.Exceptions;
using Serilog;
using StackExchange.Redis;

namespace Catalog.Api;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureLogger(builder);

        var services = builder.Services;
        {
            ConfigureHealthChecks(services, builder.Configuration);

            ConfigureDatabase(services, builder.Configuration);
            ConfigureCache(services, builder.Configuration);

            ConfigureMassTransit(services, builder.Configuration);
            ConfigureBlob(services, builder.Configuration);

            ConfigureSwagger(services);
            ConfigureMappers(services);

            services.AddControllers();
            services.AddEndpointsApiExplorer();
        }

        var app = builder.Build();
        {
            var runner = app.Services.GetRequiredService<MigrationRunner>();
            runner.RunMigrations();

            app.UseExceptionHandlingMiddleware();
            app.UseSerilogRequestLogging();

            app.MapControllers();

            ConfigureSwaggerUI(app);
            ConfigureHealthChecksEndpoints(app);
        }

        app.Run();
    }

    private static void ConfigureLogger(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((hostingContext, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .Enrich.WithThreadId();
        });
    }

    private static void ConfigureHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddNpgSql(
                    npgsqlConnectionString: configuration["PostgresConnectionString"]!,
                    name: "PostgresSQL",
                    failureStatus: HealthStatus.Unhealthy)
                .AddRedis(
                    redisConnectionString: configuration["RedisConnectionString"]!,
                    name: "Redis",
                    failureStatus: HealthStatus.Unhealthy)
                .AddMinio(
                    factory: sp => sp.GetRequiredService<MinioClient>(),
                    name: "Minio",
                    failureStatus: HealthStatus.Unhealthy)
                .AddRabbitMQ(
                    rabbitConnectionString: configuration["RabbitMqConnectionString"]!,
                    name: "RabbitMq",
                    failureStatus: HealthStatus.Unhealthy);

        services.AddHealthChecksUI()
                .AddInMemoryStorage();
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DapperDbContext>(_ => new DapperDbContext(configuration["PostgresConnectionString"]!));
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IProductImageRepository, ProductImageRepository>();
        services.AddTransient<IBrandRepository, BrandRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();

        services.AddSingleton<MigrationRunner>(serviceProvider =>
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<MigrationRunner>();
            return new MigrationRunner(logger, configuration["PostgresConnectionString"]!);
        });
    }

    private static void ConfigureCache(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]!));
        services.AddSingleton<RedisCacheManager>();
        services.Decorate<IProductRepository, CachedProductRepository>();
        services.Decorate<IProductImageRepository, CachedProductImageRepository>();
        services.Decorate<IBrandRepository, CachedBrandRepository>();
        services.Decorate<ICategoryRepository, CachedCategoryRepository>();
    }

    private static void ConfigureMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumers(Assembly.GetExecutingAssembly());

            configurator.UsingRabbitMq((context, busConfigurator) =>
            {
                busConfigurator.ConfigureEndpoints(context);

                configurator.AddConsumers(Assembly.GetExecutingAssembly());

                busConfigurator.Host(configuration["RabbitMqConnectionString"]);

                busConfigurator.UseRetry(retryConfig =>
                {
                    retryConfig.Interval(10, TimeSpan.FromSeconds(30));
                    retryConfig.Handle<BrokerUnreachableException>();
                });
            });
        });
    }

    private static void ConfigureBlob(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMinio(options =>
        {
            options.Endpoint = configuration["MinioOptions:Endpoint"]!;
            options.AccessKey = configuration["MinioOptions:AccessKey"]!;
            options.SecretKey = configuration["MinioOptions:SecretKey"]!;
        });

        var blobServiceSettings = configuration.GetSection("MinioBlobServiceSettings").Get<BlobServiceSettings>()!;
        var imageHandlingSettings = configuration.GetSection("ImageHandlingSettings").Get<ImageHandlingSettings>()!;

        services.AddSingleton<IBlobServiceSettings, BlobServiceSettings>(_ => blobServiceSettings);
        services.AddSingleton<IImageHandlingSettings, ImageHandlingSettings>(_ => imageHandlingSettings);

        services.AddTransient<IBlobStorage, MinioBlobStorage>();
        services.AddTransient<IBlobService, BlobService>();
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API of Catalog microservice",
                Version = "v1",
            });

            var filePath = Path.Combine(AppContext.BaseDirectory, "Catalog.Api.xml");
            options.IncludeXmlComments(filePath);
        });
    }

    private static void ConfigureMappers(IServiceCollection services)
    {
        services.AddSingleton<IBrandMapper, BrandMapper>();
        services.AddSingleton<ICategoryMapper, CategoryMapper>();
        services.AddSingleton<IProductImageMapper, ProductImageMapper>();
        services.AddSingleton<IProductMapper, ProductMapper>();
    }

    private static void ConfigureSwaggerUI(IApplicationBuilder app)
    {
        app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
        app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1"); });
    }

    private static void ConfigureHealthChecksEndpoints(IApplicationBuilder app)
    {
        app.UseHealthChecks("/healthz", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecksUI(config =>
        {
            config.UIPath = "/health-ui";
            config.ApiPath = "/health-ui-api";
        });
    }
}