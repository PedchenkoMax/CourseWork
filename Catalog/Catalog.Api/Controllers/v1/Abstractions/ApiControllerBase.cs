using System.Diagnostics;
using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public class ApiControllerBase<TController> : ControllerBase where TController : ControllerBase
{
    protected BadRequestObjectResult BadRequest(ValidationResult validationResult)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status400BadRequest);

        problemDetails.Extensions["errors"] = validationResult
                                              .Errors
                                              .ToLookup(f => f.PropertyName, f => f.ErrorMessage)
                                              .ToDictionary(group => group.Key, group => group.ToArray());

        return BadRequest(problemDetails);
    }

    protected NotFoundObjectResult NotFound(string fieldName)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status404NotFound);

        problemDetails.Extensions["errors"] = new Dictionary<string, string[]>
            { { fieldName, new[] { "The requested resource was not found." } } };

        return NotFound(problemDetails);
    }

    protected new OkObjectResult Ok(object data)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status200OK);

        problemDetails.Extensions["data"] = data;

        return base.Ok(problemDetails);
    }

    private ProblemDetails CreateProblemDetails(int status)
    {
        var problemDetails = new ProblemDetails
        {
            Status = status,
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            },
        };

        return problemDetails;
    }
}