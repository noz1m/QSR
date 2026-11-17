using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Features.Cash;
using QSR.Domain.Entities.Branches.Cash;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchPos")]
[ApiController]
[Route("api/cash-shifts")]
public class CashShiftsController(CashShiftHandler handler, BranchIdentity branch) : ControllerBase
{
    private string Actor => User.FindFirst("sub")?.Value
        ?? throw new InvalidOperationException("Authenticated subject is required.");

    [HttpPost]
    public async Task<IActionResult> Open(OpenShiftRequest request, CancellationToken cancellationToken)
    {
        return Ok(await handler.OpenAsync(branch.Id, request.RegisterCode,
            request.Currency, request.OpeningAmount, Actor, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await handler.GetAsync(branch.Id, id, cancellationToken));
    }

    [HttpPost("{id:guid}/entries")]
    public async Task<IActionResult> Record(Guid id, CashEntryRequest request,
        CancellationToken cancellationToken)
    {
        await handler.RecordAsync(branch.Id, id, request.Key, request.Type,
            request.Amount, Actor, request.Reason, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CloseShiftRequest request,
        CancellationToken cancellationToken)
    {
        await handler.CloseAsync(branch.Id, id, request.CountedAmount, Actor, cancellationToken);
        return Ok(await handler.GetAsync(branch.Id, id, cancellationToken));
    }
}

public sealed record OpenShiftRequest(string RegisterCode, string Currency, decimal OpeningAmount);
public sealed record CashEntryRequest(Guid Key, CashEntryType Type, decimal Amount, string Reason);
public sealed record CloseShiftRequest(decimal CountedAmount);
