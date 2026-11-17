using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using QSR.Application.Features.Inventory.GetLowStockIngredients;

namespace QSR.WebApi.BackgroundJobs;

public sealed class LowStockSnapshotJob(
    IServiceScopeFactory scopeFactory,
    IDistributedCache cache,
    ILogger<LowStockSnapshotJob> logger) : BackgroundService
{
    public const string CacheKey = "inventory:low-stock:v1";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        do
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var handler = scope.ServiceProvider
                    .GetRequiredService<GetLowStockIngredientsHandler>();
                var snapshot = await handler.HandleAsync(stoppingToken);

                await cache.SetStringAsync(
                    CacheKey,
                    JsonSerializer.Serialize(snapshot),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    },
                    stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Could not refresh the low stock snapshot.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
