using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Api.Swagger.Examples.Brokers;
using RealEstate.Api.Common;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Brokers.Contracts;
using RealEstate.Application.Features.Brokers.Create;
using RealEstate.Application.Features.Brokers.GetById;
using RealEstate.Application.Features.Brokers.List;
using RealEstate.Application.Features.Brokers.Update;

namespace RealEstate.Api.Controllers;

[ApiController]
[Route("api/brokers")]
public sealed class BrokersController : ControllerBase
{
    private readonly IBrokerService _service;
    private const string EntityName = "Broker";

    public BrokersController(IBrokerService service)
    {
        _service = service;
    }

    // GET /api/brokers?brokerId=&firstName=&lastName=&agencyId=&email=&phoneNumber=&page=&pageSize=&sortBy=&sortDirection=
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BrokerListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BrokerListResponseExample))]
    public async Task<ActionResult<PagedResult<BrokerListItemDto>>> GetList(
        [FromQuery] BrokerListQuery query,
        CancellationToken ct)
    {
        var result = await _service.GetListAsync(query, ct);
        return Ok(result);
    }

    // GET /api/brokers/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BrokerDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BrokerDetailsResponseExample))]
    public async Task<ActionResult<BrokerDetailsDto>> GetById(string id, CancellationToken ct)
    {
        var bad = this.ParseIdOrProblem(id, EntityName, out var brokerId);
        if (bad is not null) return bad;

        var dto = await _service.GetByIdAsync(brokerId, ct);
        if (dto is null) return this.EntityNotFound(EntityName, id);

        return Ok(dto);
    }

    // POST /api/brokers
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    [ProducesResponseType(typeof(BrokerDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BrokerDetailsDto>> Create(
        [FromBody] CreateBrokerRequest request,
        CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    // PUT /api/brokers/{id}
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BrokerDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BrokerDetailsDto>> Update(
        string id,
        [FromBody] UpdateBrokerRequest request,
        CancellationToken ct)
    {
        var bad = this.ParseIdOrProblem(id, EntityName, out var brokerId);
        if (bad is not null) return bad;

        var updated = await _service.UpdateAsync(brokerId, request, ct);
        if (updated is null) return this.EntityNotFound(EntityName, id);

        return Ok(updated);
    }

    // DELETE /api/brokers/{id}
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var bad = this.ParseIdOrProblem(id, EntityName, out var brokerId);
        if (bad is not null) return bad;

        var deleted = await _service.DeleteAsync(brokerId, ct);
        if (!deleted) return this.EntityNotFound(EntityName, id);

        return NoContent();
    }
}
