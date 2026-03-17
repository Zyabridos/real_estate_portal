using Microsoft.AspNetCore.Mvc;

namespace RealEstate.Api.Common;

public static class ApiProblemDetailsExtensions
{
    public static ObjectResult? ParseIdOrProblem(
        this ControllerBase controller,
        string rawId,
        string entityName,
        out int id)
    {
        id = default;

        if (string.IsNullOrWhiteSpace(rawId) || !int.TryParse(rawId, out id) || id <= 0)
        {
            return controller.BadRequest(new ProblemDetails
            {
                Title = "Invalid id",
                Detail = $"{entityName} id must be a positive integer.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        return null;
    }

    public static NotFoundObjectResult EntityNotFound(
        this ControllerBase controller,
        string entityName,
        object id)
    {
        return controller.NotFound(new ProblemDetails
        {
            Title = $"{entityName} not found",
            Detail = $"{entityName} with id '{id}' was not found.",
            Status = StatusCodes.Status404NotFound
        });
    }
}