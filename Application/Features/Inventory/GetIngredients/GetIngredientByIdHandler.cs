using QSR.Application.Interfaces.Repositories.Inventory;

namespace QSR.Application.Features.Inventory.GetIngredients;

public class GetIngredientByIdHandler(
    IIngredientRepository ingredientRepository)
{
    public async Task<IngredientResult> HandleAsync(
        Guid ingredientId,
        CancellationToken cancellationToken = default)
    {
        if (ingredientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Ingredient ID is required.");
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

        return new IngredientResult(
            ingredient.Id,
            ingredient.Name,
            ingredient.CurrentStock,
            ingredient.MinimumStock,
            ingredient.UnitOfMeasureCode,
            ingredient.IsActive,
            ingredient.IsLowStock);
    }
}