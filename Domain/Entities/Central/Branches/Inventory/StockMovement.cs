
using QSR.Domain.Common;
using QSR.Domain.Enums;

namespace QSR.Domain.Entities.Branches.Inventory;

public class StockMovement : AuditableEntity
{
    public Guid WarehouseId { get; private set; }

    public Guid IngredientId { get; private set; }

    public StockMovementType Type { get; private set; }

    public decimal Quantity { get; private set; }

    public string UnitOfMeasureCode { get; private set; } = null!;

    public Guid? OrderId { get; private set; }

    public Guid? OrderLineId { get; private set; }

    public Guid? ReversesMovementId { get; private set; }

    public string IdempotencyKey { get; private set; } = null!;

    private StockMovement()
    {
    }

    public static StockMovement Create(
        Guid warehouseId,
        Guid ingredientId,
        StockMovementType type,
        decimal quantity,
        string unitOfMeasureCode,
        string idempotencyKey,
        Guid? orderId = null,
        Guid? orderLineId = null,
        Guid? reversesMovementId = null)
    {
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse ID cannot be empty.");

        if (ingredientId == Guid.Empty)
            throw new ArgumentException("Ingredient ID cannot be empty.");

        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid stock movement type.");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException("Unit of measure code is required.");

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("Idempotency key is required.");

        if (orderId.HasValue != orderLineId.HasValue)
            throw new ArgumentException(
                "Order ID and Order Line ID must be provided together.");

        if (reversesMovementId == Guid.Empty)
            throw new ArgumentException(
                "Reversed movement ID cannot be empty.");

        return new StockMovement
        {
            WarehouseId = warehouseId,
            IngredientId = ingredientId,
            Type = type,
            Quantity = quantity,
            UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant(),
            IdempotencyKey = idempotencyKey.Trim(),
            OrderId = orderId,
            OrderLineId = orderLineId,
            ReversesMovementId = reversesMovementId
        };
    }
}
