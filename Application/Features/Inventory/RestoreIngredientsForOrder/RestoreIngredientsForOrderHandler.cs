using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Inventory.RestoreIngredientsForOrder;

public class RestoreIngredientsForOrderHandler(
    IIngredientRepository ingredientRepository,
    IInventoryMovementRepository inventoryMovementRepository)
{
    public async Task HandleAsync(
        Guid orderId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID is required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "Refund reason is required.");
        }

        var movements =
            await inventoryMovementRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        var consumptions = movements
            .Where(x =>
                x.Type == InventoryMovementType.OrderConsumption)
            .ToArray();

        if (consumptions.Length == 0)
        {
            throw new InvalidOperationException(
                "No inventory consumption was found for this order.");
        }

        var refunds = movements
            .Where(x =>
                x.Type == InventoryMovementType.Refund)
            .ToArray();

        var ingredientIds = consumptions
            .Select(x => x.IngredientId)
            .Distinct()
            .ToArray();

        var ingredients =
            await ingredientRepository.GetByIdsAsync(
                ingredientIds,
                cancellationToken);

        var ingredientDictionary = ingredients
            .ToDictionary(x => x.Id);

        var refundMovements =
            new List<InventoryMovement>();

        foreach (var ingredientId in ingredientIds)
        {
            if (!ingredientDictionary.TryGetValue(
                    ingredientId,
                    out var ingredient))
            {
                throw new InvalidOperationException(
                    $"Ingredient {ingredientId} was not found.");
            }

            var ingredientConsumptions = consumptions
                .Where(x =>
                    x.IngredientId == ingredientId)
                .ToArray();

            var units = ingredientConsumptions
                .Select(x => x.UnitOfMeasureCode)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (units.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Ingredient {ingredientId} has different units of measure.");
            }

            var consumedQuantity =
                ingredientConsumptions.Sum(x => x.Quantity);

            var alreadyRefundedQuantity = refunds
                .Where(x =>
                    x.IngredientId == ingredientId)
                .Sum(x => x.Quantity);

            var quantityToRestore =
                consumedQuantity - alreadyRefundedQuantity;

            if (quantityToRestore < 0)
            {
                throw new InvalidOperationException(
                    $"Refunded quantity exceeds consumed quantity for ingredient {ingredient.Name}.");
            }

            if (quantityToRestore == 0)
            {
                continue;
            }

            if (!string.Equals(
                    ingredient.UnitOfMeasureCode,
                    units[0],
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Unit of measure mismatch for ingredient {ingredient.Name}.");
            }

            ingredient.AddStock(
                quantityToRestore);

            var refundMovement =
                InventoryMovement.Create(
                    ingredient.Id,
                    InventoryMovementType.Refund,
                    quantityToRestore,
                    ingredient.UnitOfMeasureCode,
                    orderId,
                    reason);

            refundMovements.Add(
                refundMovement);
        }

        if (refundMovements.Count == 0)
        {
            throw new InvalidOperationException(
                "Inventory for this order has already been fully restored.");
        }

        await inventoryMovementRepository.AddRangeAsync(
            refundMovements,
            cancellationToken);

        await ingredientRepository.SaveChangesAsync(
            cancellationToken);
    }
}