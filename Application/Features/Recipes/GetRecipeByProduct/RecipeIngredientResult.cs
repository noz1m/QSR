namespace QSR.Application.Features.Recipes.GetRecipeByProduct;

public sealed record RecipeIngredientResult(
    Guid BranchIngredientId,
    decimal Quantity,
    string UnitOfMeasureCode);