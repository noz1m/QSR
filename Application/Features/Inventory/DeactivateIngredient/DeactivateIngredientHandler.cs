using QSR.Application.Interfaces.Repositories.Inventory;

namespace QSR.Application.Features.Inventory.DeactivateIngredient;

public class DeactivateIngredientHandler(
    IIngredientRepository ingredientRepository)
{
    public async Task HandleAsync(
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

        if (!ingredient.IsActive)
        {
            return;
        }

        ingredient.Deactivate();

        await ingredientRepository.SaveChangesAsync(
            cancellationToken);
    }
}