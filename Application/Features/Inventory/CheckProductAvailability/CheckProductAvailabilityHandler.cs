using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Application.Interfaces.Repositories.Recipes;

namespace QSR.Application.Features.Inventory.CheckProductAvailability;

public class CheckProductAvailabilityHandler(
    IBranchRecipeRepository branchRecipeRepository,
    IIngredientRepository ingredientRepository)
{
    public async Task<ProductAvailabilityResult> HandleAsync(
        Guid branchProductId,
        int quantity = 1,
        CancellationToken cancellationToken = default)
    {
        if (branchProductId == Guid.Empty)
            throw new ArgumentException(
                "Branch product ID is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var recipe =
            await branchRecipeRepository.GetByBranchProductIdAsync(
                branchProductId,
                cancellationToken);

        if (recipe is null)
        {
            throw new InvalidOperationException(
                "Recipe was not found for this product.");
        }

        if (!recipe.IsActive)
        {
            throw new InvalidOperationException(
                "Recipe is not active.");
        }

        var ingredientIds = recipe.Ingredients
            .Select(x => x.BranchIngredientId)
            .Distinct()
            .ToArray();

        var ingredients =
            await ingredientRepository.GetByIdsAsync(
                ingredientIds,
                cancellationToken);

        var ingredientDictionary = ingredients
            .ToDictionary(x => x.Id);

        var shortages =
            new List<IngredientShortageResult>();

        foreach (var recipeIngredient in recipe.Ingredients)
        {
            if (!ingredientDictionary.TryGetValue(
                    recipeIngredient.BranchIngredientId,
                    out var ingredient))
            {
                throw new InvalidOperationException(
                    $"Ingredient {recipeIngredient.BranchIngredientId} was not found.");
            }

            if (!ingredient.IsActive)
            {
                throw new InvalidOperationException(
                    $"Ingredient {ingredient.Name} is not active.");
            }

            if (!string.Equals(
                    ingredient.UnitOfMeasureCode,
                    recipeIngredient.UnitOfMeasureCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Unit of measure mismatch for ingredient {ingredient.Name}.");
            }

            var requiredQuantity =
                recipeIngredient.Quantity * quantity;

            if (ingredient.CurrentStock < requiredQuantity)
            {
                shortages.Add(
                    new IngredientShortageResult(
                        ingredient.Id,
                        ingredient.Name,
                        requiredQuantity,
                        ingredient.CurrentStock,
                        ingredient.UnitOfMeasureCode));
            }
        }

        return new ProductAvailabilityResult(
            branchProductId,
            shortages.Count == 0,
            shortages);
    }
}