using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Api.Swagger.Examples.Agencies;
using RealEstate.Api.Common;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Agencies.Contracts;
using RealEstate.Application.Features.Agencies.Create;
using RealEstate.Application.Features.Agencies.GetById;
using RealEstate.Application.Features.Agencies.List;
using RealEstate.Application.Features.Agencies.Update;

namespace RealEstate.Api.Controllers;

[ApiController]
[Route("api/agencies")]
public sealed class AgenciesController : ControllerBase
{
    private readonly IAgencyService _service;
    private const string EntityName = "Agency";

    public AgenciesController(IAgencyService service)
    {
        _service = service;
    }

    // GET /api/agencies?id=&name=&orgNumber=&city=&page=&pageSize=&sortBy=&sortDirection=
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AgencyListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AgencyListResponseExample))]
    public async Task<ActionResult<PagedResult<AgencyListItemDto>>> GetList(
        [FromQuery] AgencyListQuery query,
        CancellationToken ct)
    {
        var result = await _service.GetListAsync(query, ct);
        return Ok(result);
    }

    // GET /api/agencies/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AgencyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AgencyDetailsResponseExample))]
    public async Task<ActionResult<AgencyDetailsDto>> GetById(string id, CancellationToken ct)
    {
        var bad = this.ParseGuidOrProblem(id, EntityName, out var guid);
        if (bad is not null) return bad;

        var dto = await _service.GetByIdAsync(guid, ct);
        if (dto is null) return this.EntityNotFound(EntityName, id);

        return Ok(dto);
    }

    // POST /api/ageincies
    [HttpPost]
    [ProducesResponseType(typeof(AgencyDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AgencyDetailsDto>> Create(
        [FromBody] CreateAgencyRequest request,
        CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id.ToString() },
            created);
    }
    
    // PUT /api/agencies/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AgencyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AgencyDetailsDto>> Update(
        string id,
        [FromBody] UpdateAgencyRequest request,
        CancellationToken ct)
    {
        var bad = this.ParseGuidOrProblem(id, EntityName, out var guid);
        if (bad is not null) return bad;

        var updated = await _service.UpdateAsync(guid, request, ct);
        if (updated is null) return this.EntityNotFound(EntityName, id);

        return Ok(updated);
    }

    // DELETE /api/agencies/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
