using QSR.Application.Interfaces.Repositories.Inventory;

namespace QSR.Application.Features.Inventory.GetIngredients;

public class GetIngredientsHandler(
    IIngredientRepository ingredientRepository)
{
    public async Task<IReadOnlyCollection<IngredientResult>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var ingredients =
            await ingredientRepository.GetAllAsync(
                cancellationToken);

        return ingredients
            .Select(x =>
                new IngredientResult(
                    x.Id,
                    x.Name,
                    x.CurrentStock,
                    x.MinimumStock,
                    x.UnitOfMeasureCode,
                    x.IsActive,
                    x.IsLowStock))
            .ToArray();
    }
}