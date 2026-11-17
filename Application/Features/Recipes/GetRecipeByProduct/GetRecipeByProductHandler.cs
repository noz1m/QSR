using QSR.Application.Interfaces.Repositories.Recipes;

namespace QSR.Application.Features.Recipes.GetRecipeByProduct;

public class GetRecipeByProductHandler(
    IBranchRecipeRepository branchRecipeRepository)
{
    public async Task<RecipeResult> HandleAsync(
        Guid branchProductId,
        CancellationToken cancellationToken = default)
    {
        if (branchProductId == Guid.Empty)
        {
            throw new ArgumentException(
                "Branch product ID is required.");
        }

        var recipe =
            await branchRecipeRepository.GetByBranchProductIdAsync(
                branchProductId,
                cancellationToken);

        if (recipe is null)
        {
            throw new InvalidOperationException(
                "Recipe was not found.");
        }

        var ingredients = recipe.Ingredients
            .Select(x =>
                new RecipeIngredientResult(
                    x.BranchIngredientId,
                    x.Quantity,
                    x.UnitOfMeasureCode))
            .ToArray();

        return new RecipeResult(
            recipe.Id,
            recipe.BranchProductId,
            recipe.IsActive,
            ingredients);
    }
}