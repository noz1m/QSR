using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Interfaces.Repositories.Kitchen;
using QSR.Application.Features.Kitchen.AcceptKitchenTask;
using QSR.Application.Features.Kitchen.StartKitchenTask;
using QSR.Application.Features.Kitchen.MarkKitchenTaskReady;
using QSR.Application.Features.Kitchen.HandOffKitchenTask;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchKitchen")]
[ApiController]
[Route("api/kitchen/tasks")]
public class KitchenTasksController(IKitchenTaskRepository repository, BranchIdentity branch) : ControllerBase
{
    [HttpPost("{id:guid}/accept")]
    public async Task<IActionResult> AcceptKitchenTask(Guid id,
        [FromServices] AcceptKitchenTaskHandler handler, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        if (task is null || task.BranchId != branch.Id)
            return NotFound();

        try
        {
            await handler.HandleAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return Conflict(new ProblemDetails { Title = "Operation conflicts with current state." });
        }
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartKitchenTask(Guid id,
        [FromServices] StartKitchenTaskHandler handler, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        if (task is null || task.BranchId != branch.Id)
            return NotFound();

        try
        {
            await handler.HandleAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return Conflict(new ProblemDetails { Title = "Operation conflicts with current state." });
        }
    }

    [HttpPost("{id:guid}/ready")]
    public async Task<IActionResult> MarkKitchenTaskReady(Guid id,
        [FromServices] MarkKitchenTaskReadyHandler handler, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        if (task is null || task.BranchId != branch.Id)
            return NotFound();

        try
        {
            await handler.HandleAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return Conflict(new ProblemDetails { Title = "Operation conflicts with current state." });
        }
    }

    [HttpPost("{id:guid}/hand-off")]
    public async Task<IActionResult> HandOffKitchenTask(Guid id,
        [FromServices] HandOffKitchenTaskHandler handler, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        if (task is null || task.BranchId != branch.Id)
            return NotFound();

        try
        {
            await handler.HandleAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return Conflict(new ProblemDetails { Title = "Operation conflicts with current state." });
        }
    }

    [HttpGet("station/{stationId:guid}")]
    public async Task<IActionResult> GetStationQueue(
        Guid stationId, CancellationToken cancellationToken)
    {
        return Ok(await repository.GetStationQueueAsync(
            branch.Id, stationId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        if (task is null || task.BranchId != branch.Id)
            return NotFound();

        return Ok(task);
    }
}
