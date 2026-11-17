using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QSR.Application.Features.Cash;
using QSR.Application.Features.Catalog;
using QSR.Application.Features.Orders.CreateOrder;
using QSR.Application.Features.Orders.SubmitOrderForPayment;
using QSR.Application.Features.Orders.AcceptOrder;
using QSR.Application.Features.Orders.MarkOrderReady;
using QSR.Application.Features.Orders.MarkOrderAwaitingPickup;
using QSR.Application.Features.Orders.CompleteOrder;
using QSR.Application.Features.Kitchen.AcceptKitchenTask;
using QSR.Application.Features.Kitchen.StartKitchenTask;
using QSR.Application.Features.Kitchen.MarkKitchenTaskReady;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Domain.Entities.Branches.Recipes;
using QSR.Domain.Enums;
using QSR.Domain.ValueObjects;
using QSR.Infrastructure.DI;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Tests;

[Collection("Postgres")]
public class OrderFlowTests
{
    [PostgresFact]
    public async Task PaidOrderConsumesStockAndCompletesAfterBothStations()
    {
        var services = new ServiceCollection();
        services.AddDbContext<BranchDbContext>(o =>
            o.UseNpgsql(Environment.GetEnvironmentVariable("QSR_TEST_POSTGRES")));
        services.AddInfrastructure();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BranchDbContext>();
        await db.Database.MigrateAsync();
        var branchId = Guid.NewGuid();
        var ingredient = Ingredient.Create("Test ingredient", "KG", 1, 10);
        db.Ingredients.Add(ingredient);
        var catalog = scope.ServiceProvider.GetRequiredService<BranchCatalogHandler>();
        var menuId = await catalog.CreateMenuAsync(branchId,
            new CreateMenuCommand(Guid.NewGuid(), "Test menu", 1), default);
        var categoryId = await catalog.CreateCategoryAsync(branchId, menuId,
            new CreateCategoryCommand(Guid.NewGuid(), "Test category", 0), default);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            catalog.CreateCategoryAsync(Guid.NewGuid(), menuId,
                new CreateCategoryCommand(Guid.NewGuid(), "Forbidden", 0), default));
        var products = new List<BranchProduct>();
        for (var i = 0; i < 2; i++)
        {
            var stationId = await catalog.CreateStationAsync(branchId,
                new CreateStationCommand($"Station {i}", $"TEST-{Guid.NewGuid():N}",
                    KitchenStationType.Grill, i), default);
            var productId = await catalog.CreateProductAsync(branchId, categoryId,
                new CreateProductCommand(Guid.NewGuid(), $"Product {i}", 10,
                    "TJS", stationId, 5, i, null), default);
            var product = await db.BranchProducts.SingleAsync(x => x.Id == productId);
            var recipe = BranchRecipe.Create(product.Id, Guid.NewGuid(), 1,
                DateTimeOffset.UtcNow.AddDays(-1));
            recipe.AddIngredient(BranchRecipeIngredient.Create(recipe.Id,
                Guid.NewGuid(), ingredient.Id, 1, "KG"));
            db.BranchRecipes.Add(recipe);
            products.Add(product);
        }
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        T Handler<T>() where T : notnull => scope.ServiceProvider.GetRequiredService<T>();

        var command = new CreateOrderCommand(branchId, null, OrderType.Pickup,
            "TJS", products.Select(p => new CreateOrderLineCommand(p.Id, null, 1, [])).ToArray());
        var key = Guid.NewGuid().ToString("N");
        var order = await Handler<CreateOrderHandler>().HandleAsync(command, creationKey: key);
        var replay = await Handler<CreateOrderHandler>().HandleAsync(command, creationKey: key);
        Assert.Equal(order.Id, replay.Id);
        await Handler<SubmitOrderForPaymentHandler>().HandleAsync(order.Id);
        var shift = await Handler<CashShiftHandler>().OpenAsync(branchId,
            Guid.NewGuid().ToString("N"), "TJS", 100, "test-cashier", default);
        var paymentKey = Guid.NewGuid();
        await Handler<CashShiftHandler>().PayAsync(branchId, shift.Id,
            order.Id, paymentKey, 20, "test-cashier", default);
        await Handler<CashShiftHandler>().PayAsync(branchId, shift.Id,
            order.Id, paymentKey, 20, "test-cashier", default);
        Assert.Equal(120, shift.ExpectedAmount);
        Assert.Single(shift.Entries);
        await Handler<AcceptOrderHandler>().HandleAsync(order.Id);
        var tasks = await db.KitchenTasks.Where(x => x.OrderId == order.Id).ToListAsync();
        Assert.Equal(2, tasks.Count);
        Assert.Equal(8, (await db.Ingredients.FindAsync(ingredient.Id))!.CurrentStock);
        foreach (var task in tasks)
        {
            await Handler<AcceptKitchenTaskHandler>().HandleAsync(task.Id);
            await Handler<StartKitchenTaskHandler>().HandleAsync(task.Id);
            await Handler<MarkKitchenTaskReadyHandler>().HandleAsync(task.Id);
            if (task == tasks[0])
                Assert.Equal(OrderStatus.Preparing, (await db.Orders.FindAsync(order.Id))!.Status);
        }
        await Handler<MarkOrderReadyHandler>().HandleAsync(order.Id);
        await Handler<MarkOrderAwaitingPickupHandler>().HandleAsync(order.Id);
        await Handler<CompleteOrderHandler>().HandleAsync(order.Id);
        db.ChangeTracker.Clear();
        Assert.Equal(OrderStatus.Completed, (await db.Orders.FindAsync(order.Id))!.Status);
        Assert.Equal(8, (await db.Ingredients.FindAsync(ingredient.Id))!.CurrentStock);
    }
}

[CollectionDefinition("Postgres", DisableParallelization = true)]
public class PostgresCollection;
