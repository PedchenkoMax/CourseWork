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
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Minio;
using Minio.AspNetCore;
using Minio.AspNetCore.HealthChecks;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId();
});

var configuration = builder.Configuration;

var services = builder.Services;
{
    services.AddSingleton<DapperDbContext>(_ => new DapperDbContext(configuration["PostgresConnectionString"]!));
    services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]!));
    services.AddSingleton<RedisCacheManager>();

    services.AddTransient<IProductRepository, ProductRepository>();
    services.Decorate<IProductRepository, CachedProductRepository>();

    services.AddTransient<IProductImageRepository, ProductImageRepository>();
    services.Decorate<IProductImageRepository, CachedProductImageRepository>();
    
    services.AddTransient<IBrandRepository, BrandRepository>();
    services.Decorate<IBrandRepository, CachedBrandRepository>();
    
    services.AddTransient<ICategoryRepository, CategoryRepository>();
    services.Decorate<ICategoryRepository, CachedCategoryRepository>();

    services.AddMinio(options =>
    {
        options.Endpoint = configuration["MinioOptions:Endpoint"]!;
        options.AccessKey = configuration["MinioOptions:AccessKey"]!;
        options.SecretKey = configuration["MinioOptions:SecretKey"]!;
    });

    services.AddHealthChecks()
            .AddNpgSql(
                npgsqlConnectionString: configuration["PostgresConnectionString"],
                name: "PostgresSQL",
                failureStatus: HealthStatus.Unhealthy)
            .AddRedis(
                redisConnectionString: configuration["RedisConnectionString"],
                name: "Redis",
                failureStatus: HealthStatus.Unhealthy)
            .AddMinio(
                factory: sp => sp.GetRequiredService<MinioClient>(),
                name: "Minio",
                failureStatus: HealthStatus.Unhealthy)
            .AddRabbitMQ(
                rabbitConnectionString: configuration["RabbitMqConnectionString"],
                name: "RabbitMq",
                failureStatus: HealthStatus.Unhealthy);

    services.AddHealthChecksUI()
            .AddInMemoryStorage();

    var blobServiceSettings = configuration.GetSection("MinioBlobServiceSettings").Get<BlobServiceSettings>()!;
    services.AddSingleton<IBlobServiceSettings, BlobServiceSettings>(_ => blobServiceSettings);

    var imageHandlingSettings = configuration.GetSection("ImageHandlingSettings").Get<ImageHandlingSettings>()!;
    services.AddSingleton<IImageHandlingSettings, ImageHandlingSettings>(_ => imageHandlingSettings);

    services.AddTransient<IBlobStorage, MinioBlobStorage>();
    services.AddTransient<IBlobService, BlobService>();

    var migrationRunner = new MigrationRunner();
    migrationRunner.RunMigrations(configuration["PostgresConnectionString"]!, true);

    services.AddSingleton<IBrandMapper, BrandMapper>();
    services.AddSingleton<ICategoryMapper, CategoryMapper>();
    services.AddSingleton<IProductImageMapper, ProductImageMapper>();
    services.AddSingleton<IProductMapper, ProductMapper>();
    
    services.AddControllers();
    services.AddEndpointsApiExplorer();

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


var app = builder.Build();
{
    app.UseSerilogRequestLogging(options =>
    {
    });
    
    app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1"); });

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

    app.UseExceptionHandlingMiddleware();

    app.MapControllers();
}

app.Run();