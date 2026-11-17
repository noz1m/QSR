using QSR.Domain.Common;
using QSR.Domain.Enums;

namespace QSR.Domain.Entities.Branches.Inventory;

public class InventoryMovement : AuditableEntity
{
    public Guid IngredientId { get; private set; }

    public Guid? OrderId { get; private set; }

    public InventoryMovementType Type { get; private set; }

    public decimal Quantity { get; private set; }

    public string UnitOfMeasureCode { get; private set; } =
        string.Empty;

    public string? Reason { get; private set; }

    private InventoryMovement()
    {
    }

    public static InventoryMovement Create(
        Guid ingredientId,
        InventoryMovementType type,
        decimal quantity,
        string unitOfMeasureCode,
        Guid? orderId = null,
        string? reason = null)
    {
        if (ingredientId == Guid.Empty)
            throw new ArgumentException(
                "Ingredient ID is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException(
                "Unit of measure is required.");

        if (type == InventoryMovementType.OrderConsumption &&
            !orderId.HasValue)
        {
            throw new ArgumentException(
                "Order ID is required for order consumption.");
        }

        return new InventoryMovement
        {
            IngredientId = ingredientId,
            OrderId = orderId,
            Type = type,
            Quantity = quantity,
            UnitOfMeasureCode =
                unitOfMeasureCode.Trim().ToUpperInvariant(),
            Reason = reason?.Trim()
        };
    }
}