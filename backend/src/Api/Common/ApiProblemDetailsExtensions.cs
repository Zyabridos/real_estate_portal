using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RealEstate.Api.Common;

public static class ApiProblemDetailsExtensions
{
    public static ObjectResult InvalidGuidId(this ControllerBase controller, string entityName, string rawId) =>
        controller.Problem(
            title: "Invalid id",
            detail: $"{entityName} id must be a valid GUID. Value was '{rawId}'.",
            statusCode: StatusCodes.Status400BadRequest);

    public static ObjectResult EntityNotFound(this ControllerBase controller, string entityName, string rawId) =>
        controller.Problem(
            title: "Not found",
            detail: $"{entityName} '{rawId}' was not found.",
            statusCode: StatusCodes.Status404NotFound);
    
    public static ObjectResult? ParseGuidOrProblem(
        this ControllerBase controller,
        string rawId,
        string entityName,
        out Guid id)
    {
        if (Guid.TryParse(rawId, out id))
            return null;

        id = default;
        return controller.InvalidGuidId(entityName, rawId);
    }
}