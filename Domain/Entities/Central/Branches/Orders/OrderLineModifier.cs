using QSR.Domain.Common;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Branches.Orders;

public class OrderLineModifier : BaseEntity
{
    public Guid OrderLineId { get; private set; }

    public Guid BranchModifierOptionId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Money PriceAdjustment { get; private set; }

    private OrderLineModifier()
    {
    }

    public static OrderLineModifier Create(
        Guid orderLineId,
        Guid branchModifierOptionId,
        string name,
        Money priceAdjustment)
    {
        if (orderLineId == Guid.Empty)
            throw new ArgumentException(
                "Order line ID is required.");

        if (branchModifierOptionId == Guid.Empty)
            throw new ArgumentException(
                "Branch modifier option ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Modifier name is required.");

        if (priceAdjustment.Amount < 0)
            throw new ArgumentException(
                "Price adjustment cannot be negative.");

        if (string.IsNullOrWhiteSpace(priceAdjustment.Currency))
            throw new ArgumentException(
                "Currency is required.");

        return new OrderLineModifier
        {
            OrderLineId = orderLineId,
            BranchModifierOptionId = branchModifierOptionId,
            Name = name.Trim(),
            PriceAdjustment = priceAdjustment
        };
    }
}