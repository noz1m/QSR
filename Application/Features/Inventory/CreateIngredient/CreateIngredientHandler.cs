using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Inventory.CreateIngredient;

public class CreateIngredientHandler(
    IIngredientRepository ingredientRepository,
    IInventoryMovementRepository inventoryMovementRepository)
{
    public async Task<CreateIngredientResult> HandleAsync(
        CreateIngredientCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException(
                "Ingredient name is required.");
        }

        if (string.IsNullOrWhiteSpace(
                command.UnitOfMeasureCode))
        {
            throw new ArgumentException(
                "Unit of measure is required.");
        }

        if (command.MinimumStock < 0)
        {
            throw new ArgumentException(
                "Minimum stock cannot be negative.");
        }

        if (command.InitialStock < 0)
        {
            throw new ArgumentException(
                "Initial stock cannot be negative.");
        }

        var ingredient = Ingredient.Create(
            command.Name,
            command.UnitOfMeasureCode,
            command.MinimumStock,
            command.InitialStock);

        await ingredientRepository.AddAsync(
            ingredient,
            cancellationToken);

        if (command.InitialStock > 0)
        {
            var movement = InventoryMovement.Create(
                ingredient.Id,
                InventoryMovementType.StockIn,
                command.InitialStock,
                ingredient.UnitOfMeasureCode,
                reason: "Initial stock");

            await inventoryMovementRepository.AddAsync(
                movement,
                cancellationToken);
        }

        await ingredientRepository.SaveChangesAsync(
            cancellationToken);

        return new CreateIngredientResult(
            ingredient.Id,
            ingredient.Name,
            ingredient.CurrentStock,
            ingredient.MinimumStock,
            ingredient.UnitOfMeasureCode,
            ingredient.IsActive);
    }
}