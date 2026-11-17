using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Application.Interfaces.Repositories.Menu;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Interfaces.Repositories.Recipes;
using QSR.Application.Services.Recipes;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Entities.Branches.Menu;

namespace QSR.Application.Features.Inventory.CheckOrderInventoryAvailability;

public class CheckOrderInventoryAvailabilityHandler(
    IOrderRepository orderRepository,
    IBranchProductRepository branchProductRepository,
    IBranchRecipeRepository branchRecipeRepository,
    IIngredientRepository ingredientRepository,
    RecipeConsumptionCalculator recipeConsumptionCalculator)
{
    public async Task<OrderInventoryAvailabilityResult> HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID is required.");
        }

        var order = await orderRepository.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null)
        {
            throw new InvalidOperationException(
                "Order was not found.");
        }

        if (order.Lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Order does not contain any lines.");
        }

        var productIds = order.Lines
            .Select(x => x.BranchProductId)
            .Distinct()
            .ToArray();

        var products =
            await branchProductRepository.GetByIdsAsync(
                productIds,
                cancellationToken);

        var productDictionary = products
            .ToDictionary(x => x.Id);

        if (productDictionary.Count != productIds.Length)
        {
            throw new InvalidOperationException(
                "One or more products were not found.");
        }

        var recipes =
            await branchRecipeRepository.GetByBranchProductIdsAsync(
                productIds,
                cancellationToken);

        var recipeDictionary = recipes
            .ToDictionary(x => x.BranchProductId);

        var allConsumptions =
            new List<IngredientConsumption>();

        foreach (var orderLine in order.Lines)
        {
            if (!productDictionary.TryGetValue(
                    orderLine.BranchProductId,
                    out var product))
            {
                throw new InvalidOperationException(
                    $"Product {orderLine.BranchProductId} was not found.");
            }

            if (!recipeDictionary.TryGetValue(
                    orderLine.BranchProductId,
                    out var recipe))
            {
                throw new InvalidOperationException(
                    $"Recipe was not found for product {product.Name}.");
            }

            if (!recipe.IsActive)
            {
                throw new InvalidOperationException(
                    $"Recipe for product {product.Name} is not active.");
            }

            var selectedModifiers = GetSelectedModifiers(
                product,
                orderLine.Modifiers
                    .Select(x => x.BranchModifierOptionId));

            var consumptions =
                recipeConsumptionCalculator.Calculate(
                    recipe,
                    selectedModifiers);

            foreach (var consumption in consumptions)
            {
                var quantity =
                    consumption.Quantity *
                    orderLine.Quantity;

                if (quantity <= 0)
                    continue;

                allConsumptions.Add(
                    new IngredientConsumption(
                        consumption.IngredientId,
                        quantity,
                        consumption.UnitOfMeasureCode));
            }
        }

        var groupedConsumptions = allConsumptions
            .GroupBy(x => x.IngredientId)
            .Select(group =>
            {
                var units = group
                    .Select(x => x.UnitOfMeasureCode)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (units.Length != 1)
                {
                    throw new InvalidOperationException(
                        $"Ingredient {group.Key} uses different units of measure.");
                }

                return new IngredientConsumption(
                    group.Key,
                    group.Sum(x => x.Quantity),
                    units[0]);
            })
            .ToArray();

        var ingredientIds = groupedConsumptions
            .Select(x => x.IngredientId)
            .ToArray();

        var ingredients =
            await ingredientRepository.GetByIdsAsync(
                ingredientIds,
                cancellationToken);

        var ingredientDictionary = ingredients
            .ToDictionary(x => x.Id);

        var shortages =
            new List<OrderIngredientShortageResult>();

        foreach (var consumption in groupedConsumptions)
        {
            if (!ingredientDictionary.TryGetValue(
                    consumption.IngredientId,
                    out var ingredient))
            {
                throw new InvalidOperationException(
                    $"Ingredient {consumption.IngredientId} was not found.");
            }

            if (!ingredient.IsActive)
            {
                shortages.Add(
                    new OrderIngredientShortageResult(
                        ingredient.Id,
                        ingredient.Name,
                        consumption.Quantity,
                        ingredient.CurrentStock,
                        ingredient.UnitOfMeasureCode));

                continue;
            }

            if (!string.Equals(
                    ingredient.UnitOfMeasureCode,
                    consumption.UnitOfMeasureCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Unit of measure mismatch for ingredient {ingredient.Name}.");
            }

            if (ingredient.CurrentStock <
                consumption.Quantity)
            {
                shortages.Add(
                    new OrderIngredientShortageResult(
                        ingredient.Id,
                        ingredient.Name,
                        consumption.Quantity,
                        ingredient.CurrentStock,
                        ingredient.UnitOfMeasureCode));
            }
        }

        return new OrderInventoryAvailabilityResult(
            order.Id,
            shortages.Count == 0,
            shortages);
    }

    private static IReadOnlyCollection<BranchModifierOption>
        GetSelectedModifiers(
            BranchProduct product,
            IEnumerable<Guid> selectedModifierIds)
    {
        var modifierIds = selectedModifierIds
            .ToHashSet();

        if (modifierIds.Count == 0)
        {
            return Array.Empty<BranchModifierOption>();
        }

        var modifiers = product.ModifierGroups
            .SelectMany(x => x.Options)
            .Where(x => modifierIds.Contains(x.Id))
            .ToArray();

        if (modifiers.Length != modifierIds.Count)
        {
            throw new InvalidOperationException(
                "One or more selected modifiers were not found.");
        }

        return modifiers;
    }
}