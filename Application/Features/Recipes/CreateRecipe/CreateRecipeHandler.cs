using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Application.Interfaces.Repositories.Menu;
using QSR.Application.Interfaces.Repositories.Recipes;
using QSR.Domain.Entities.Branches.Recipes;

namespace QSR.Application.Features.Recipes.CreateRecipe;

public sealed record CreateRecipeCommand(
    Guid BranchProductId, Guid SourceTemplateId, int Version,
    DateTimeOffset EffectiveFrom, DateTimeOffset? EffectiveTo,
    IReadOnlyCollection<CreateRecipeIngredient> Ingredients);

public sealed record CreateRecipeIngredient(
    Guid SourceTemplateId, Guid IngredientId, decimal Quantity, string UnitOfMeasureCode);

public class CreateRecipeHandler(
    IBranchRecipeRepository recipes,
    IBranchProductRepository products,
    IIngredientRepository ingredients)
{
    public async Task<Guid> HandleAsync(
        CreateRecipeCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Ingredients is null || command.Ingredients.Count is 0 or > 200 ||
            command.Ingredients.Any(x => x is null))
            throw new ArgumentException("Recipe must contain between 1 and 200 ingredients.");

        if (command.Ingredients.Select(x => x.IngredientId).Distinct().Count() !=
            command.Ingredients.Count)
            throw new ArgumentException("Ingredient cannot be repeated.");

        var recipe = BranchRecipe.Create(command.BranchProductId,
            command.SourceTemplateId, command.Version,
            command.EffectiveFrom, command.EffectiveTo);

        var product = await products.GetByIdAsync(command.BranchProductId, cancellationToken)
            ?? throw new ArgumentException("Product was not found.");

        var selectedIngredients = await ingredients.GetByIdsAsync(
            command.Ingredients.Select(x => x.IngredientId), cancellationToken);
        var byId = selectedIngredients.ToDictionary(x => x.Id);

        foreach (var line in command.Ingredients)
        {
            if (!byId.TryGetValue(line.IngredientId, out var ingredient) ||
                !ingredient.IsActive)
                throw new ArgumentException("An active ingredient is required.");

            if (!string.Equals(line.UnitOfMeasureCode?.Trim(),
                    ingredient.UnitOfMeasureCode, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Recipe and stock units must match.");

            if (decimal.Round(line.Quantity, 3) != line.Quantity)
                throw new ArgumentException("Ingredient quantity supports three decimal places.");

            recipe.AddIngredient(BranchRecipeIngredient.Create(recipe.Id,
                line.SourceTemplateId, ingredient.Id, line.Quantity, ingredient.UnitOfMeasureCode));
        }

        var existing = await recipes.GetVersionAsync(
            product.Id, command.Version, cancellationToken);

        if (existing is not null)
        {
            var same = existing.SourceRecipeTemplateId == recipe.SourceRecipeTemplateId &&
                existing.EffectiveFrom == recipe.EffectiveFrom &&
                existing.EffectiveTo == recipe.EffectiveTo &&
                existing.Ingredients.Count == recipe.Ingredients.Count &&
                existing.Ingredients.All(x => recipe.Ingredients.Any(y =>
                    x.BranchIngredientId == y.BranchIngredientId &&
                    x.SourceRecipeIngredientTemplateId == y.SourceRecipeIngredientTemplateId &&
                    x.Quantity == y.Quantity &&
                    x.UnitOfMeasureCode == y.UnitOfMeasureCode));

            if (!same)
                throw new RecipeVersionConflictException();

            return existing.Id;
        }

        await recipes.AddAsync(recipe, cancellationToken);
        await recipes.SaveChangesAsync(cancellationToken);
        return recipe.Id;
    }
}

public sealed class RecipeVersionConflictException()
    : Exception("Recipe version already contains different data.");
