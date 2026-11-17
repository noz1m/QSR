namespace QSR.Application.Features.Inventory.UpdateIngredient;

public sealed record UpdateIngredientResult(
    Guid Id,
    string Name,
    decimal CurrentStock,
    decimal MinimumStock,
    string UnitOfMeasureCode,
    bool IsActive);