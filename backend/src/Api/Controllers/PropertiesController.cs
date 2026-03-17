using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Api.Swagger.Examples.Properties;
using RealEstate.Api.Common;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Properties.Contracts;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Application.Features.Properties.Update;

namespace RealEstate.Api.Controllers;

[ApiController]
[Route("api/properties")]
public sealed class PropertiesController : ControllerBase
{
    private readonly IPropertyService _service;
    private const string EntityName = "Property";

    public PropertiesController(IPropertyService service)
    {
        _service = service;
    }

    // GET /api/properties?city=&type=&status=&minPrice=&maxPrice=&page=&pageSize=&sort=
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PropertyListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(StatusCodes.Status200OK, typeof(PropertyListResponseExample))]
    public async Task<ActionResult<PagedResult<PropertyListItemDto>>> GetList(
        [FromQuery] PropertyListQuery query,
        CancellationToken ct)
    {
        // Validation of query is done by FluentValidation va filter/auto-validation.
        var result = await _service.GetListAsync(query, ct);
        return Ok(result);
    }

    // GET /api/properties/{id}
    [HttpGet("{id}")]
	[ProducesResponseType(typeof(PropertyDetailsDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(StatusCodes.Status200OK, typeof(PropertyDetailsResponseExample))]
	public async Task<ActionResult<PropertyDetailsDto>> GetById(string id, CancellationToken ct)
	{
        var bad = this.ParseIdOrProblem(id, EntityName, out var propertyId);
        if (bad is not null) return bad;

        var dto = await _service.GetByIdAsync(propertyId, ct);
        if (dto is null) return this.EntityNotFound(EntityName, id);

        return Ok(dto);
}

    // POST /api/properties
    [HttpPost]
    [ProducesResponseType(typeof(PropertyDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropertyDetailsDto>> Create(
        [FromBody] CreatePropertyRequest request,
        CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);

		return CreatedAtAction(
		nameof(GetById),
		new { id = created.Id.ToString() },
		created);
    }

    // PUT /api/properties/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PropertyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropertyDetailsDto>> Update(
        string id,
        [FromBody] UpdatePropertyRequest request,
        CancellationToken ct)
    {
        var bad = this.ParseIdOrProblem(id, EntityName, out var propertyId);
        if (bad is not null) return bad;

        var updated = await _service.UpdateAsync(propertyId, request, ct);
        if (updated is null) return this.EntityNotFound(EntityName, id);

        return Ok(updated);
    }

    // DELETE /api/properties/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var bad = this.ParseIdOrProblem(id, EntityName, out var propertyId);
        if (bad is not null) return bad;

        var deleted = await _service.DeleteAsync(propertyId, ct);
        if (!deleted) return this.EntityNotFound(EntityName, id);

        return NoContent();
    }
}
