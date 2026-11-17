using QSR.Domain.Entities.Branches.Inventory;

namespace QSR.Application.Interfaces.Repositories.Inventory;

public interface IInventoryMovementRepository
{
    Task<IReadOnlyCollection<InventoryMovement>>
        GetByIngredientIdAsync(
            Guid ingredientId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryMovement>>
        GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

    Task AddAsync(
        InventoryMovement movement,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<InventoryMovement> movements,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}