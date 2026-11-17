namespace QSR.Application.Features.Inventory.CheckOrderInventoryAvailability;

public sealed record OrderIngredientShortageResult(
    Guid IngredientId,
    string IngredientName,
    decimal RequiredQuantity,
    decimal AvailableQuantity,
    string UnitOfMeasureCode);

public sealed record OrderInventoryAvailabilityResult(
    Guid OrderId,
    bool IsAvailable,
    IReadOnlyCollection<OrderIngredientShortageResult> Shortages);