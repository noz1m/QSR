using QSR.Application.Interfaces.Repositories.Inventory;

namespace QSR.Application.Features.Inventory.GetInventoryMovements;

public class GetInventoryMovementsByIngredientHandler(
    IInventoryMovementRepository inventoryMovementRepository)
{
    public async Task<IReadOnlyCollection<InventoryMovementResult>> HandleAsync(
        Guid ingredientId,
        CancellationToken cancellationToken = default)
    {
        if (ingredientId == Guid.Empty)
            throw new ArgumentException(
                "Ingredient ID is required.");

        var movements =
            await inventoryMovementRepository.GetByIngredientIdAsync(
                ingredientId,
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