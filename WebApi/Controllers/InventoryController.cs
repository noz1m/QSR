using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using QSR.Application.Features.Inventory.GetLowStockIngredients;
using QSR.WebApi.BackgroundJobs;
using StackExchange.Redis;

namespace QSR.WebApi.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController(
    GetLowStockIngredientsHandler handler,
    IServiceProvider services,
    ILogger<InventoryController> logger) : ControllerBase
{
    [Authorize(Policy = "BranchInventory")]
    [HttpGet("low-stock")]
    public async Task<IReadOnlyCollection<LowStockIngredientResult>> GetLowStock(
        CancellationToken cancellationToken)
    {
        if (services.GetService<IDistributedCache>() is { } cache)
        {
            try
            {
                var json = await cache.GetStringAsync(
                    LowStockSnapshotJob.CacheKey, cancellationToken);

                if (json is not null &&
                    JsonSerializer.Deserialize<LowStockIngredientResult[]>(json) is { } snapshot)
                    return snapshot;
            }
            catch (Exception exception) when (exception is RedisException or JsonException)
            {
                logger.LogWarning(exception, "Could not read the low stock snapshot.");
            }
        }

        return await handler.HandleAsync(cancellationToken);
    }
}
