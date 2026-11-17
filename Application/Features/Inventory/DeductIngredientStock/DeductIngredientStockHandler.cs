using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Inventory.DeductIngredientStock;

public class DeductIngredientStockHandler(
    IIngredientRepository ingredientRepository,
    IInventoryMovementRepository inventoryMovementRepository)
{
    public async Task HandleAsync(
        Guid orderId,
        IReadOnlyCollection<IngredientConsumption> consumptions,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        ArgumentNullException.ThrowIfNull(consumptions);

        if (consumptions.Count == 0)
        {
            throw new InvalidOperationException(
                "Ingredient consumptions are required.");
        }

        var groupedConsumptions = consumptions
            .GroupBy(x => x.IngredientId)
            .Select(group =>
                new IngredientConsumption(
                    group.Key,
                    group.Sum(x => x.Quantity),
                    group.First().UnitOfMeasureCode))
            .ToArray();

        var ingredientIds = groupedConsumptions
            .Select(x => x.IngredientId)
            .ToArray();

        var ingredients =
            await ingredientRepository.GetByIdsAsync(
                ingredientIds,
                cancellationToken);

        var ingredientDictionary = ingredients
            .ToDictionary(x => x.Id);

        var movements = new List<InventoryMovement>();

        foreach (var consumption in groupedConsumptions)
        {
            if (!ingredientDictionary.TryGetValue(
                    consumption.IngredientId,
                    out var ingredient))
            {
                throw new InvalidOperationException(
                    $"Ingredient {consumption.IngredientId} was not found.");
            }

            if (!ingredient.IsActive)
            {
                throw new InvalidOperationException(
                    $"Ingredient {ingredient.Name} is not active.");
            }

            if (!string.Equals(
                    ingredient.UnitOfMeasureCode,
                    consumption.UnitOfMeasureCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Unit of measure does not match for ingredient {ingredient.Name}.");
            }

            ingredient.DeductStock(
                consumption.Quantity);

            var movement = InventoryMovement.Create(
                ingredient.Id,
                InventoryMovementType.OrderConsumption,
                consumption.Quantity,
                ingredient.UnitOfMeasureCode,
                orderId);

            movements.Add(movement);
        }

        await inventoryMovementRepository.AddRangeAsync(
            movements,
            cancellationToken);

        await ingredientRepository.SaveChangesAsync(
            cancellationToken);
    }
}