using QSR.Application.Interfaces.Repositories.Inventory;

namespace QSR.Application.Features.Inventory.GetLowStockIngredients;

public class GetLowStockIngredientsHandler(
    IIngredientRepository ingredientRepository)
{
    public async Task<IReadOnlyCollection<LowStockIngredientResult>>
        HandleAsync(
            CancellationToken cancellationToken = default)
    {
        var ingredients =
            await ingredientRepository.GetLowStockAsync(
                cancellationToken);

        return ingredients
            .Select(x =>
                new LowStockIngredientResult(
                    x.Id,
                    x.Name,
                    x.CurrentStock,
                    x.MinimumStock,
                    Math.Max(
                        0,
                        x.MinimumStock - x.CurrentStock),
                    x.UnitOfMeasureCode))
            .ToArray();
    }
}