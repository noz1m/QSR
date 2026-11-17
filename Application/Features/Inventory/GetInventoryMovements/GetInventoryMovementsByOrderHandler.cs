using QSR.Application.Interfaces.Repositories.Inventory;

namespace QSR.Application.Features.Inventory.GetInventoryMovements;

public class GetInventoryMovementsByOrderHandler(
    IInventoryMovementRepository inventoryMovementRepository)
{
    public async Task<IReadOnlyCollection<InventoryMovementResult>> HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        var movements =
            await inventoryMovementRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        return movements
            .Select(x =>
                new InventoryMovementResult(
                    x.Id,
                    x.IngredientId,
                    x.OrderId,
                    x.Type,
                    x.Quantity,
                    x.UnitOfMeasureCode,
                    x.Reason,
                    x.CreatedAt))
            .ToArray();
    }
}