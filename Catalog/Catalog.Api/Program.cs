using Catalog.Api.Middlewares;
using Catalog.Api.Services;
using Catalog.Api.Services.Abstractions;
using Catalog.Infrastructure.BlobStorage;
using Catalog.Infrastructure.BlobStorage.Abstractions;
using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Minio.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var services = builder.Services;
{
    services.AddSingleton<DapperDbContext>(_ => new DapperDbContext(configuration["ConnectionString"]!));
    services.AddTransient<IProductRepository, ProductRepository>();
    services.AddTransient<IProductImageRepository, ProductImageRepository>();
    services.AddTransient<IBrandRepository, BrandRepository>();
    services.AddTransient<ICategoryRepository, CategoryRepository>();

    services.AddMinio(options =>
    {
        options.Endpoint = configuration["MinioOptions:Endpoint"]!;
        options.AccessKey = configuration["MinioOptions:AccessKey"]!;
        options.SecretKey = configuration["MinioOptions:SecretKey"]!;
    });

    var blobServiceSettings = configuration.GetSection("MinioBlobServiceSettings").Get<BlobServiceSettings>()!;
    services.AddSingleton<IBlobServiceSettings, BlobServiceSettings>(_ => blobServiceSettings);

    var imageHandlingSettings = configuration.GetSection("ImageHandlingSettings").Get<ImageHandlingSettings>()!;
    services.AddSingleton<IImageHandlingSettings, ImageHandlingSettings>(_ => imageHandlingSettings);

    services.AddTransient<IBlobStorage, MinioBlobStorage>();
    services.AddTransient<IBlobService, BlobService>();

    var migrationRunner = new MigrationRunner();
    migrationRunner.RunMigrations(configuration["ConnectionString"]!, true);

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
    app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1"); });

    app.UseExceptionHandlingMiddleware();

    app.MapControllers();
}

app.Run();