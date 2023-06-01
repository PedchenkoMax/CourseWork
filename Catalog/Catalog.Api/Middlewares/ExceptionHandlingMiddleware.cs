using System.Text.Json;
using Catalog.Infrastructure.Database.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Minio.Exceptions;

namespace Catalog.Api.Middlewares;

public static class ExceptionHandlingMiddleware
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                try
                {
                    throw context.Features.Get<IExceptionHandlerFeature>()?.Error!;
                }
                catch (DatabaseException)
                {
                    await HandleException(context, $"An error occurred while querying the database");
                }
                catch (MinioException)
                {
                    await HandleException(context, $"An error occurred with Minio.");
                }
                catch (Exception)
                {
                    await HandleException(context, $"An error has occurred.");
                }
            });
        });

        return app;
    }

    private static async Task HandleException(HttpContext context, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "Server Error",
            Title = "Server Error",
            Detail = detail
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(problemDetails);

        await context.Response.WriteAsync(json);
    }
}