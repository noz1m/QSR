using QSR.Application.Features.Inventory.DeductIngredientStock;
using QSR.Application.Interfaces.Repositories.Menu;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Interfaces.Repositories.Recipes;
using QSR.Application.Services.Recipes;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Domain.Entities.Branches.Menu;

namespace QSR.Application.Features.Inventory.DeductIngredientsForOrder;

public class DeductIngredientsForOrderHandler(
    IOrderRepository orderRepository,
    IBranchProductRepository branchProductRepository,
    IBranchRecipeRepository branchRecipeRepository,
    RecipeConsumptionCalculator recipeConsumptionCalculator,
    DeductIngredientStockHandler deductIngredientStockHandler)
{
    public async Task HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        var order = await orderRepository.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                "Order was not found.");

        if (order.Lines.Count == 0)
            throw new InvalidOperationException(
                "Order does not contain any lines.");

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

            var selectedModifiers =
                GetSelectedModifiers(
                    product,
                    orderLine.Modifiers
                        .Select(x => x.BranchModifierOptionId));

            var consumptions =
                recipeConsumptionCalculator.Calculate(
                    recipe,
                    selectedModifiers);

            foreach (var consumption in consumptions)
            {
                allConsumptions.Add(
                    new IngredientConsumption(
                        consumption.IngredientId,
                        consumption.Quantity *
                        orderLine.Quantity,
                        consumption.UnitOfMeasureCode));
            }
        }

        await deductIngredientStockHandler.HandleAsync(
            order.Id,
            allConsumptions,
            cancellationToken);
    }

    private static IReadOnlyCollection<BranchModifierOption>
        GetSelectedModifiers(
            BranchProduct product,
            IEnumerable<Guid> selectedModifierIds)
    {
        var modifierIds = selectedModifierIds
            .ToHashSet();

        if (modifierIds.Count == 0)
            return Array.Empty<BranchModifierOption>();

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