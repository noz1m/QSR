namespace QSR.Application.Features.Inventory.CheckProductAvailability;

public sealed record IngredientShortageResult(
    Guid IngredientId,
    string Name,
    decimal RequiredQuantity,
    decimal AvailableQuantity,
    string UnitOfMeasureCode);

public sealed record ProductAvailabilityResult(
    Guid BranchProductId,
    bool IsAvailable,
    IReadOnlyCollection<IngredientShortageResult> Shortages);