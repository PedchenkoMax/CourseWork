using System.Diagnostics;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.Abstractions;

public class ApiControllerBase<TController> : ControllerBase where TController : ControllerBase
{
    protected BadRequestObjectResult BadRequest(ValidationResult validationResult)
    {
        const string type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        const string title = "One or more validation errors occurred.";
        const int status = StatusCodes.Status400BadRequest;

        var errors = validationResult
                     .Errors
                     .ToLookup(f => f.PropertyName, f => f.ErrorMessage)
                     .ToDictionary(group => group.Key, group => group.ToArray());

        var problemDetails = CreateProblemDetails(type, title, status, errors);

        return BadRequest(problemDetails);
    }

    protected NotFoundObjectResult NotFound(string fieldName)
    {
        const string type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        const string title = "Not Found";
        const int status = StatusCodes.Status404NotFound;

        var errors = new Dictionary<string, string[]> { { fieldName, new[] { "The requested resource was not found." } } };

        var problemDetails = CreateProblemDetails(type, title, status, errors);

        return NotFound(problemDetails);
    }

    private ProblemDetails CreateProblemDetails(string type, string title, int status, object errors)
    {
        return new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = status,
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id ?? HttpContext?.TraceIdentifier,
                ["errors"] = errors,
            },
        };
    }
}