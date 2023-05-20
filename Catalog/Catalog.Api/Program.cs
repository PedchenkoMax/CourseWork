using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var services = builder.Services;
{
    services.AddSingleton<DbContext>(_ => new DbContext(configuration["ConnectionString"]!));
    services.AddTransient<IProductRepository, ProductRepository>();
    services.AddTransient<IProductImageRepository, ProductImageRepository>();
    services.AddTransient<IBrandRepository, BrandRepository>();
    services.AddTransient<ICategoryRepository, CategoryRepository>();
    
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(options =>
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Catalog.Api.xml");
        options.IncludeXmlComments(filePath);
    });
}


var app = builder.Build();
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapControllers();
}

app.Run();