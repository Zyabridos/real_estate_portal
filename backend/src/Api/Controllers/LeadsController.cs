using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Api.Swagger.Examples.Leads;
using RealEstate.Application.Common;
using RealEstate.Api.Common;
using RealEstate.Application.Features.Leads.Contracts;
using RealEstate.Application.Features.Leads.Create;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Application.Features.Leads.Update;

using RealEstate.Application.Features.Leads.GetById;

namespace RealEstate.Api.Controllers;

[ApiController]
[Route("api/leads")]
public sealed class LeadsController : ControllerBase
{
    private readonly ILeadService _service;
    private const string EntityName = "Lead";

    public LeadsController(ILeadService service)
    {
        _service = service;
    }

    // GET /api/leads?id=&propertyId=&fullName=&email=&phoneNumber=&page=&pageSize=&sortBy=&sortDirection=
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LeadListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LeadListResponseExample))]
    public async Task<ActionResult<PagedResult<LeadListItemDto>>> GetList(
        [FromQuery] LeadListQuery query,
        CancellationToken ct)
    {
        var result = await _service.GetListAsync(query, ct);
        return Ok(result);
    }

    // GET /api/leads/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LeadDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LeadDetailsResponseExample))]
    public async Task<ActionResult<LeadDetailsDto>> GetById(string id, CancellationToken ct)
    {
        var bad = this.ParseGuidOrProblem(id, EntityName, out var guid);
        if (bad is not null) return bad;

        var dto = await _service.GetByIdAsync(guid, ct);
        if (dto is null) return this.EntityNotFound(EntityName, id);

        return Ok(dto);
    }

    // POST /api/leads
    [HttpPost]
    [ProducesResponseType(typeof(LeadDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeadDetailsDto>> Create(
        [FromBody] CreateLeadRequest request,
        CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id.ToString() },
            created);
    }

    // PUT /api/leads/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(LeadDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeadDetailsDto>> Update(
        string id,
        [FromBody] UpdateLeadRequest request,
        CancellationToken ct)
    {
        var bad = this.ParseGuidOrProblem(id, EntityName, out var guid);
        if (bad is not null) return bad;

        var updated = await _service.UpdateAsync(guid, request, ct);
        if (updated is null) return this.EntityNotFound(EntityName, id);

        return Ok(updated);
    }

    // DELETE /api/leads/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var bad = this.ParseGuidOrProblem(id, EntityName, out var guid);
        if (bad is not null) return bad;

        var deleted = await _service.DeleteAsync(guid, ct);
        if (!deleted) return this.EntityNotFound(EntityName, id);

        return NoContent();
    }
}
