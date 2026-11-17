using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branch.Inventory;

public class StockBalance : AuditableEntity
{
    public Guid WarehouseId { get; private set; }

    public Guid IngredientId { get; private set; }

    public decimal QuantityOnHand { get; private set; }

    public decimal ReservedQuantity { get; private set; }

    public decimal AvailableQuantity =>
        QuantityOnHand - ReservedQuantity;

    private StockBalance() { }

    public static StockBalance Create(
        Guid warehouseId,
        Guid ingredientId)
    {
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse ID is required.");

        if (ingredientId == Guid.Empty)
            throw new ArgumentException("Ingredient ID is required.");

        return new StockBalance
        {
            WarehouseId = warehouseId,
            IngredientId = ingredientId,
            QuantityOnHand = 0,
            ReservedQuantity = 0
        };
    }

    public void Receive(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Received quantity must be greater than zero.");

        QuantityOnHand += quantity;
    }

    public void Reserve(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Reserved quantity must be greater than zero.");

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException(
                "Insufficient available stock.");

        ReservedQuantity += quantity;
    }

    public void ReleaseReservation(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Released quantity must be greater than zero.");

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException(
                "Released quantity exceeds reserved stock.");

        ReservedQuantity -= quantity;
    }

    public void Deduct(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Deducted quantity must be greater than zero.");

        if (QuantityOnHand < quantity)
            throw new InvalidOperationException(
                "Insufficient stock.");

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException(
                "Cannot deduct reserved stock.");

        QuantityOnHand -= quantity;
    }

    public void DeductReserved(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Deducted quantity must be greater than zero.");

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException(
                "Deducted quantity exceeds reserved stock.");

        if (QuantityOnHand < quantity)
            throw new InvalidOperationException(
                "Insufficient stock.");

        QuantityOnHand -= quantity;
        ReservedQuantity -= quantity;
    }

    public void Adjust(decimal newQuantity)
    {
        if (newQuantity < 0)
            throw new ArgumentException(
                "Stock quantity cannot be negative.");

        if (newQuantity < ReservedQuantity)
            throw new InvalidOperationException(
                "Stock cannot be lower than reserved quantity.");

        QuantityOnHand = newQuantity;
    }
}