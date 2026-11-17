using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Inventory;

public class InventoryMovementRepository(
    BranchDbContext dbContext)
    : IInventoryMovementRepository
{
    public async Task<IReadOnlyCollection<InventoryMovement>>
        GetByIngredientIdAsync(
            Guid ingredientId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryMovements
            .Where(x => x.IngredientId == ingredientId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryMovement>>
        GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryMovements
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        InventoryMovement movement,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(movement);

        await dbContext.InventoryMovements.AddAsync(
            movement,
            cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<InventoryMovement> movements,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(movements);

        await dbContext.InventoryMovements.AddRangeAsync(
            movements,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}