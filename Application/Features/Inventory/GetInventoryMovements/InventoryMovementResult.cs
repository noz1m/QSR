using QSR.Domain.Enums;

namespace QSR.Application.Features.Inventory.GetInventoryMovements;

public sealed record InventoryMovementResult(
    Guid Id,
    Guid IngredientId,
    Guid? OrderId,
    InventoryMovementType Type,
    decimal Quantity,
    string UnitOfMeasureCode,
    string? Reason,
    DateTimeOffset CreatedAt);