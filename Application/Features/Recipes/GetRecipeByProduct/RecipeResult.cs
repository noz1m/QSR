namespace QSR.Application.Features.Recipes.GetRecipeByProduct;

public sealed record RecipeResult(
    Guid Id,
    Guid BranchProductId,
    bool IsActive,
    IReadOnlyCollection<RecipeIngredientResult> Ingredients);