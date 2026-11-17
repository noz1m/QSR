using Microsoft.EntityFrameworkCore;
using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Domain.Entities.Branches.Orders;
using QSR.Domain.Entities.Branches.Payments;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Entities.Branches.Recipes;

namespace QSR.Infrastructure.Persistence.Branches;

public class BranchDbContext(
    DbContextOptions<BranchDbContext> options)
    : DbContext(options)
{
    public DbSet<BranchProduct> BranchProducts =>
        Set<BranchProduct>();

    public DbSet<Order> Orders =>
        Set<Order>();

    public DbSet<OrderLine> OrderLines =>
        Set<OrderLine>();

    public DbSet<OrderLineModifier> OrderLineModifiers =>
        Set<OrderLineModifier>();

    public DbSet<KitchenStation> KitchenStations =>
        Set<KitchenStation>();

    public DbSet<KitchenTask> KitchenTasks =>
        Set<KitchenTask>();

    public DbSet<KitchenTaskItem> KitchenTaskItems =>
        Set<KitchenTaskItem>();

    public DbSet<Payment> Payments =>
        Set<Payment>();

    public DbSet<Ingredient> Ingredients =>
        Set<Ingredient>();

    public DbSet<InventoryMovement> InventoryMovements =>
        Set<InventoryMovement>();

    public DbSet<BranchRecipe> BranchRecipes =>
        Set<BranchRecipe>();

    public DbSet<BranchRecipeIngredient> BranchRecipeIngredients =>
        Set<BranchRecipeIngredient>();

    public DbSet<PaymentRefund> PaymentRefunds =>
        Set<PaymentRefund>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BranchProduct>().ComplexProperty(x => x.CentralPrice);
        modelBuilder.Entity<BranchProduct>().ComplexProperty(x => x.LocalPrice);
        modelBuilder.Entity<BranchProductVariant>().ComplexProperty(x => x.CentralPrice);
        modelBuilder.Entity<BranchProductVariant>().ComplexProperty(x => x.LocalPrice);
        modelBuilder.Entity<BranchModifierOption>().ComplexProperty(x => x.CentralPriceAdjustment);
        modelBuilder.Entity<BranchModifierOption>().ComplexProperty(x => x.LocalPriceAdjustment);
        modelBuilder.Entity<Ingredient>().Property<uint>("Version").IsRowVersion();
        modelBuilder.Entity<Order>().Property<uint>("Version").IsRowVersion();
        modelBuilder.Entity<KitchenTask>().Property<uint>("Version").IsRowVersion();
        modelBuilder.Entity<Payment>().ToTable("payments");
        modelBuilder.Entity<Payment>().ComplexProperty(x => x.Amount, money =>
        {
            money.Property(x => x.Amount).HasPrecision(18, 2);
            money.Property(x => x.Currency).HasMaxLength(3);
        });
        modelBuilder.Entity<Order>().HasMany(x => x.Payments).WithOne()
            .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Payment>().Property<uint>("Version").IsRowVersion();
        modelBuilder.Entity<Payment>().Property(x => x.ExternalTransactionId).HasMaxLength(200);
        modelBuilder.Entity<Payment>().HasIndex(x => x.ExternalTransactionId).IsUnique();
        modelBuilder.Entity<PaymentRefund>().HasIndex(x => x.ExternalTransactionId).IsUnique();

        modelBuilder.Entity<Ingredient>().Property(x => x.CurrentStock).HasPrecision(18, 3);
        modelBuilder.Entity<Ingredient>().Property(x => x.MinimumStock).HasPrecision(18, 3);
        modelBuilder.Entity<Ingredient>().Property(x => x.Name).HasMaxLength(200);
        modelBuilder.Entity<Ingredient>().Property(x => x.UnitOfMeasureCode).HasMaxLength(20);
        modelBuilder.Entity<BranchRecipeIngredient>().Property(x => x.Quantity).HasPrecision(18, 3);
        modelBuilder.Entity<BranchRecipeIngredient>().Property(x => x.UnitOfMeasureCode).HasMaxLength(20);

        modelBuilder.Entity<BranchRecipe>()
            .HasIndex(x => new { x.BranchProductId, x.Version })
            .IsUnique();

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BranchDbContext).Assembly,
            type =>
                type.Namespace != null &&
                type.Namespace.StartsWith(
                    "QSR.Infrastructure.Persistence.Branches.Configurations"));
    }
}