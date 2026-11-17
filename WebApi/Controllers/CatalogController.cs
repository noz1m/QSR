using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Features.Catalog;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchCatalog")]
[ApiController]
[Route("api/catalog")]
public class CatalogController(
    BranchCatalogHandler handler,
    IBranchCatalogRepository repository,
    BranchIdentity branch) : ControllerBase
{
    [HttpGet("menus")]
    public async Task<IActionResult> GetMenus(CancellationToken cancellationToken)
    {
        return Ok(await repository.GetMenusAsync(branch.Id, cancellationToken));
    }

    [HttpPost("menus")]
    public async Task<IActionResult> CreateMenu(
        CreateMenuCommand command, CancellationToken cancellationToken)
    {
        return Ok(new { Id = await handler.CreateMenuAsync(branch.Id, command, cancellationToken) });
    }

    [HttpPost("menus/{menuId:guid}/categories")]
    public async Task<IActionResult> CreateCategory(Guid menuId,
        CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        return Ok(new { Id = await handler.CreateCategoryAsync(
            branch.Id, menuId, command, cancellationToken) });
    }

    [HttpPost("categories/{categoryId:guid}/products")]
    public async Task<IActionResult> CreateProduct(Guid categoryId,
        CreateProductCommand command, CancellationToken cancellationToken)
    {
        return Ok(new { Id = await handler.CreateProductAsync(
            branch.Id, categoryId, command, cancellationToken) });
    }

    [HttpGet("stations")]
    public async Task<IActionResult> GetStations(CancellationToken cancellationToken)
    {
        return Ok(await repository.GetStationsAsync(branch.Id, cancellationToken));
    }

    [HttpPost("stations")]
    public async Task<IActionResult> CreateStation(
        CreateStationCommand command, CancellationToken cancellationToken)
    {
        return Ok(new { Id = await handler.CreateStationAsync(branch.Id, command, cancellationToken) });
    }
}
