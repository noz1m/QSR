using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Features.Orders.CreateOrder;
using QSR.Application.Features.Orders.GetOrderById;

namespace QSR.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController(
    CreateOrderHandler createOrderHandler,
    GetOrderByIdHandler getOrderByIdHandler,
    BranchIdentity branch)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateOrderResult>> Create(
        [FromBody] CreateOrderCommand command,
        [FromHeader(Name = "Idempotency-Key")] Guid? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (command.BranchId != branch.Id)
            return Forbid();

        if (idempotencyKey is null || idempotencyKey == Guid.Empty)
            return BadRequest(new ProblemDetails { Title = "Idempotency-Key must be a UUID." });

        var result = await createOrderHandler.HandleAsync(
            command,
            cancellationToken,
            idempotencyKey.Value.ToString("N"));

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetOrderByIdResult>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await getOrderByIdHandler.HandleAsync(
            id,
            cancellationToken);

        if (result is null || result.BranchId != branch.Id)
            return NotFound();

        return Ok(result);
    }
}