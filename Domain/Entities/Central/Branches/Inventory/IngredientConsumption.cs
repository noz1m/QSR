namespace QSR.Domain.Entities.Branches.Inventory;

public sealed record IngredientConsumption(
    Guid IngredientId,
    decimal Quantity,
    string UnitOfMeasureCode);