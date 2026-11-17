namespace QSR.Application.Features.Inventory.GetLowStockIngredients;

public sealed record LowStockIngredientResult(
    Guid Id,
    string Name,
    decimal CurrentStock,
    decimal MinimumStock,
    decimal MissingQuantity,
    string UnitOfMeasureCode);