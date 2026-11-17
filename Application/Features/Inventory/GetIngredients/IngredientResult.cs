namespace QSR.Application.Features.Inventory.GetIngredients;

public sealed record IngredientResult(
    Guid Id,
    string Name,
    decimal CurrentStock,
    decimal MinimumStock,
    string UnitOfMeasureCode,
    bool IsActive,
    bool IsLowStock);