using QSR.Application.Interfaces.Repositories.Inventory;

namespace QSR.Application.Features.Inventory.UpdateIngredient;

public class UpdateIngredientHandler(
    IIngredientRepository ingredientRepository)
{
    public async Task<UpdateIngredientResult> HandleAsync(
        UpdateIngredientCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.IngredientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Ingredient ID is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException(
                "Ingredient name is required.");
        }

        if (command.MinimumStock < 0)
        {
            throw new ArgumentException(
                "Minimum stock cannot be negative.");
        }

        var ingredient =
            await ingredientRepository.GetByIdAsync(
                command.IngredientId,
                cancellationToken);

        if (ingredient is null)
        {
            throw new InvalidOperationException(
                "Ingredient was not found.");
        }

        ingredient.Rename(
            command.Name);

        ingredient.UpdateMinimumStock(
            command.MinimumStock);

        await ingredientRepository.SaveChangesAsync(
            cancellationToken);

        return new UpdateIngredientResult(
            ingredient.Id,
            ingredient.Name,
            ingredient.CurrentStock,
            ingredient.MinimumStock,
            ingredient.UnitOfMeasureCode,
            ingredient.IsActive);
    }
}