using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Inventory.AdjustIngredientStock;

public class AdjustIngredientStockHandler(
    IIngredientRepository ingredientRepository,
    IInventoryMovementRepository inventoryMovementRepository)
{
    public async Task HandleAsync(
        Guid ingredientId,
        decimal quantityDifference,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (ingredientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Ingredient ID is required.");
        }

        if (quantityDifference == 0)
        {
            throw new ArgumentException(
                "Quantity difference cannot be zero.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "Adjustment reason is required.");
        }

        var ingredient =
            await ingredientRepository.GetByIdAsync(
                ingredientId,
                cancellationToken);

        if (ingredient is null)
        {
            throw new InvalidOperationException(
                "Ingredient was not found.");
        }

        if (!ingredient.IsActive)
        {
            throw new InvalidOperationException(
                "Ingredient is not active.");
        }

        if (quantityDifference > 0)
        {
            ingredient.AddStock(
                quantityDifference);
        }
        else
        {
            ingredient.DeductStock(
                Math.Abs(quantityDifference));
        }

        var movement =
            InventoryMovement.Create(
                ingredient.Id,
                InventoryMovementType.Adjustment,
                Math.Abs(quantityDifference),
                ingredient.UnitOfMeasureCode,
                reason: reason);

        await inventoryMovementRepository.AddAsync(
            movement,
            cancellationToken);

        await ingredientRepository.SaveChangesAsync(
            cancellationToken);
    }
}