using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Kitchen;

public class KitchenTaskItem : BaseEntity
{
    public Guid KitchenTaskId { get; private set; }

    public Guid OrderLineId { get; private set; }

    public Guid BranchProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    private KitchenTaskItem()
    {
    }

    public static KitchenTaskItem Create(
        Guid kitchenTaskId,
        Guid orderLineId,
        Guid branchProductId,
        string productName,
        int quantity)
    {
        if (kitchenTaskId == Guid.Empty)
            throw new ArgumentException(
                "Kitchen task ID is required.");

        if (orderLineId == Guid.Empty)
            throw new ArgumentException(
                "Order line ID is required.");

        if (branchProductId == Guid.Empty)
            throw new ArgumentException(
                "Branch product ID is required.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException(
                "Product name is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        return new KitchenTaskItem
        {
            KitchenTaskId = kitchenTaskId,
            OrderLineId = orderLineId,
            BranchProductId = branchProductId,
            ProductName = productName.Trim(),
            Quantity = quantity
        };
    }
}