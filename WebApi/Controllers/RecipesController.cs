using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Features.Recipes.CreateRecipe;
using QSR.Application.Features.Recipes.GetRecipeByProduct;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchInventory")]
[ApiController]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRecipeCommand command,
        [FromServices] CreateRecipeHandler handler,
        CancellationToken cancellationToken)
    {
        var id = await handler.HandleAsync(command, cancellationToken);
        return Ok(new { Id = id });
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> Get(
        Guid productId,
        [FromServices] GetRecipeByProductHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await handler.HandleAsync(productId, cancellationToken));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
