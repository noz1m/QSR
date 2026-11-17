using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Features.Payments.AddPayment;
using QSR.Application.Features.Payments.GetPaymentsByOrder;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Features.Cash;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchPos")]
[ApiController]
[Route("api/orders/{id:guid}/payments")]
public class CashPaymentsController(
    IOrderRepository orderRepository,
    BranchIdentity branch) : ControllerBase
{
    [HttpPost("cash")]
    public async Task<IActionResult> RecordCash(
        Guid id,
        CashPaymentRequest request,
        [FromHeader(Name = "Idempotency-Key")] Guid? key,
        [FromServices] CashShiftHandler handler,
        CancellationToken cancellationToken)
    {
        if (key is null || key == Guid.Empty)
            return BadRequest(new ProblemDetails { Title = "Idempotency-Key must be a UUID." });

        try
        {
            var actor = User.FindFirst("sub")!.Value;
            return Ok(await handler.PayAsync(branch.Id, request.ShiftId,
                id, key.Value, request.Amount, actor, cancellationToken));
        }
        catch (InvalidOperationException)
        {
            return Conflict(new ProblemDetails { Title = "Payment conflicts with current state." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        Guid id,
        [FromServices] GetPaymentsByOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.BranchId != branch.Id)
            return NotFound();

        return Ok(await handler.HandleAsync(id, cancellationToken));
    }
}

public sealed record CashPaymentRequest(decimal Amount, Guid ShiftId);
