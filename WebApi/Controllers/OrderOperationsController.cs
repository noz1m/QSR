using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Features.Orders.SubmitOrderForPayment;
using QSR.Application.Features.Orders.AcceptOrder;
using QSR.Application.Features.Orders.MarkOrderReady;
using QSR.Application.Features.Orders.MarkOrderAwaitingPickup;
using QSR.Application.Features.Orders.MarkOrderOutForDelivery;
using QSR.Application.Features.Orders.CompleteOrder;
using QSR.Application.Features.Orders.CancelOrder;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchPos")]
[ApiController]
[Route("api/orders")]
public class OrderOperationsController(IOrderRepository repository, BranchIdentity branch) : ControllerBase
{
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> SubmitOrderForPayment(Guid id,
        [FromServices] SubmitOrderForPaymentHandler handler, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
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

    [HttpPost("{id:guid}/accept")]
    public async Task<IActionResult> AcceptOrder(Guid id,
        [FromServices] AcceptOrderHandler handler, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
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
    public async Task<IActionResult> MarkOrderReady(Guid id,
        [FromServices] MarkOrderReadyHandler handler, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
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

    [HttpPost("{id:guid}/awaiting-pickup")]
    public async Task<IActionResult> MarkOrderAwaitingPickup(Guid id,
        [FromServices] MarkOrderAwaitingPickupHandler handler, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
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

    [HttpPost("{id:guid}/out-for-delivery")]
    public async Task<IActionResult> MarkOrderOutForDelivery(Guid id,
        [FromServices] MarkOrderOutForDeliveryHandler handler, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
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

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteOrder(Guid id,
        [FromServices] CompleteOrderHandler handler, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
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

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id,
        [FromBody] CancelOrderRequest request,
        [FromServices] CancelOrderHandler handler, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
            return NotFound();

        try
        {
            await handler.HandleAsync(id, request.Reason, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return Conflict(new ProblemDetails { Title = "Operation conflicts with current state." });
        }
    }

}

public sealed record CancelOrderRequest(string Reason);
