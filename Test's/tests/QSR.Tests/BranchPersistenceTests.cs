using Microsoft.EntityFrameworkCore;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Entities.Branches.Orders;
using QSR.Domain.Enums;
using QSR.Infrastructure.Persistence.Branches;
using QSR.Infrastructure.Repositories.Orders;

namespace QSR.Tests;

[Collection("Postgres")]
public class BranchPersistenceTests
{
    private const string DesignConnection =
        "Host=localhost;Database=qsr;Username=qsr";

    [Fact]
    public void BranchModelMatchesMigration()
    {
        using var context = CreateContext(DesignConnection);

        Assert.NotEmpty(context.Database.GetMigrations());
        Assert.False(context.Database.HasPendingModelChanges());
        Assert.True(context.Model.FindEntityType(typeof(Ingredient))!
            .FindProperty("Version")!.IsConcurrencyToken);
    }

    [Fact]
    public void InsufficientStockDoesNotChangeBalance()
    {
        var ingredient = Ingredient.Create("Flour", "kg", 1, 2);

        Assert.Throws<InvalidOperationException>(() => ingredient.DeductStock(3));

        Assert.Equal(2m, ingredient.CurrentStock);
    }

    [PostgresFact]
    public async Task PostgresRejectsConcurrentStockUpdatesAndDuplicateOrderKeys()
    {
        var connectionString = Environment.GetEnvironmentVariable("QSR_TEST_POSTGRES");

        await using var first = CreateContext(connectionString!);
        await using var second = CreateContext(connectionString!);
        await first.Database.MigrateAsync();

        var ingredient = Ingredient.Create("Flour", "kg", 1, 10);
        first.Ingredients.Add(ingredient);
        await first.SaveChangesAsync();
        var stale = await second.Ingredients.SingleAsync(x => x.Id == ingredient.Id);

        ingredient.DeductStock(6);
        await first.SaveChangesAsync();
        stale.DeductStock(6);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => second.SaveChangesAsync());
        second.ChangeTracker.Clear();

        var branchId = Guid.NewGuid();
        var key = Guid.NewGuid().ToString("N");
        var order = Order.Create(branchId, Guid.NewGuid().ToString("N"),
            Enum.GetValues<OrderType>()[0], "TJS", creationKey: key);
        first.Orders.Add(order);
        await first.SaveChangesAsync();

        var repository = new OrderRepository(second);
        Assert.Equal(order.Id, (await repository.GetByCreationKeyAsync(branchId, key))!.Id);
        var duplicate = Order.Create(branchId, Guid.NewGuid().ToString("N"),
            Enum.GetValues<OrderType>()[0], "TJS", creationKey: key);
        second.Orders.Add(duplicate);
        await Assert.ThrowsAsync<DbUpdateException>(() => second.SaveChangesAsync());
    }

    private static BranchDbContext CreateContext(string connectionString)
    {
        return new BranchDbContext(new DbContextOptionsBuilder<BranchDbContext>()
            .UseNpgsql(connectionString)
            .Options);
    }
}

public sealed class PostgresFactAttribute : FactAttribute
{
    public PostgresFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("QSR_TEST_POSTGRES")))
            Skip = "Set QSR_TEST_POSTGRES to a disposable PostgreSQL database.";
    }
}
