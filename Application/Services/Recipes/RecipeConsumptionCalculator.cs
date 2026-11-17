using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Domain.Entities.Branches.Recipes;
using QSR.Domain.Enums;

namespace QSR.Application.Services.Recipes;

public sealed class RecipeConsumptionCalculator
{
    public IReadOnlyCollection<IngredientConsumption> Calculate(
        BranchRecipe recipe,
        IEnumerable<BranchModifierOption> selectedModifiers)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        ArgumentNullException.ThrowIfNull(selectedModifiers);

        var consumption =
            new Dictionary<Guid, IngredientConsumption>();

        foreach (var ingredient in recipe.Ingredients)
        {
            AddQuantity(
                consumption,
                ingredient.BranchIngredientId,
                ingredient.Quantity,
                ingredient.UnitOfMeasureCode);
        }

        foreach (var modifier in selectedModifiers)
        {
            foreach (var change in modifier.IngredientChanges)
            {
                var quantity = change.ChangeType switch
                {
                    IngredientChangeType.Increase =>
                        change.Quantity,

                    IngredientChangeType.Decrease =>
                        -change.Quantity,

                    _ => throw new InvalidOperationException(
                        "Unsupported ingredient change type.")
                };

                AddQuantity(
                    consumption,
                    change.BranchIngredientId,
                    quantity,
                    change.UnitOfMeasureCode);
            }
        }

        return consumption.Values
            .OrderBy(x => x.IngredientId)
            .ToArray();
    }

    private static void AddQuantity(
        Dictionary<Guid, IngredientConsumption> consumption,
        Guid ingredientId,
        decimal quantity,
        string unitOfMeasureCode)
    {
        if (consumption.TryGetValue(
                ingredientId,
                out var existing))
        {
            if (!string.Equals(
                    existing.UnitOfMeasureCode,
                    unitOfMeasureCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Cannot calculate ingredient consumption with different units.");
            }

            var newQuantity =
                existing.Quantity + quantity;

            if (newQuantity < 0)
            {
                throw new InvalidOperationException(
                    "Ingredient consumption cannot be negative.");
            }

            consumption[ingredientId] =
                existing with
                {
                    Quantity = newQuantity
                };

            return;
        }

        if (quantity < 0)
        {
            throw new InvalidOperationException(
                "Ingredient consumption cannot be negative.");
        }

        consumption.Add(
            ingredientId,
            new IngredientConsumption(
                ingredientId,
                quantity,
                unitOfMeasureCode));
    }
}