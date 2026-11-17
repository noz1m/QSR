namespace QSR.Application.Features.Inventory.CreateIngredient;

public sealed record CreateIngredientResult(
    Guid Id,
    string Name,
    decimal CurrentStock,
    decimal MinimumStock,
    string UnitOfMeasureCode,
    bool IsActive);