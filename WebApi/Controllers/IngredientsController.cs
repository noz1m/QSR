using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Features.Inventory.CreateIngredient;
using QSR.Application.Features.Inventory.GetIngredients;
using QSR.Application.Features.Inventory.UpdateIngredient;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchInventory")]
[ApiController]
[Route("api/ingredients")]
public class IngredientsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] GetIngredientsHandler handler,
        CancellationToken cancellationToken)
    {
        return Ok(await handler.HandleAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateIngredientCommand command,
        [FromServices] CreateIngredientHandler handler,
        CancellationToken cancellationToken)
    {
        return Ok(await handler.HandleAsync(command, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, UpdateIngredientCommand command,
        [FromServices] UpdateIngredientHandler handler,
        CancellationToken cancellationToken)
    {
        if (id != command.IngredientId)
            return BadRequest();

        try
        {
            return Ok(await handler.HandleAsync(command, cancellationToken));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
