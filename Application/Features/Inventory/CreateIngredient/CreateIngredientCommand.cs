namespace QSR.Application.Features.Inventory.CreateIngredient;

public sealed record CreateIngredientCommand(
    string Name,
    string UnitOfMeasureCode,
    decimal MinimumStock,
    decimal InitialStock);