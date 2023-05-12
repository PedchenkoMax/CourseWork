using Catalog.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var services = builder.Services;
{
    services.AddSingleton<DbContext>(_ => new DbContext(configuration["ConnectionString"]!));
    
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen();
}


var app = builder.Build();
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapControllers();
}

app.Run();