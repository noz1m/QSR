using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Inventory.AddIngredientStock;

public class AddIngredientStockHandler(
    IIngredientRepository ingredientRepository,
    IInventoryMovementRepository inventoryMovementRepository)
{
    public async Task HandleAsync(
        Guid ingredientId,
        decimal quantity,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (ingredientId == Guid.Empty)
            throw new ArgumentException(
                "Ingredient ID is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var ingredient =
            await ingredientRepository.GetByIdAsync(
                ingredientId,
                cancellationToken);

        if (ingredient is null)
            throw new InvalidOperationException(
                "Ingredient was not found.");

        if (!ingredient.IsActive)
            throw new InvalidOperationException(
                "Ingredient is not active.");

        ingredient.AddStock(quantity);

        var movement = InventoryMovement.Create(
            ingredient.Id,
            InventoryMovementType.StockIn,
            quantity,
            ingredient.UnitOfMeasureCode,
            reason: reason);

        await inventoryMovementRepository.AddAsync(
            movement,
            cancellationToken);

        await ingredientRepository.SaveChangesAsync(
            cancellationToken);
    }
}