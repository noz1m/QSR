using QSR.Domain.Common;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Branches.Orders;

public class OrderLine : BaseEntity
{
    private readonly List<OrderLineModifier> _modifiers = [];

    public Guid OrderId { get; private set; }

    public Guid BranchProductId { get; private set; }

    public Guid? BranchProductVariantId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public string? VariantName { get; private set; }

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    public Money TotalPrice { get; private set; }

    public IReadOnlyCollection<OrderLineModifier> Modifiers =>
        _modifiers.AsReadOnly();

    private OrderLine()
    {
    }

    public static OrderLine Create(
        Guid orderId,
        Guid branchProductId,
        string productName,
        int quantity,
        Money unitPrice,
        Guid? branchProductVariantId = null,
        string? variantName = null)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        if (branchProductId == Guid.Empty)
            throw new ArgumentException(
                "Branch product ID is required.");

        if (branchProductVariantId == Guid.Empty)
            throw new ArgumentException(
                "Branch product variant ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException(
                "Product name is required.");

        if (branchProductVariantId.HasValue &&
            string.IsNullOrWhiteSpace(variantName))
        {
            throw new ArgumentException(
                "Variant name is required when variant is selected.");
        }

        if (!branchProductVariantId.HasValue &&
            !string.IsNullOrWhiteSpace(variantName))
        {
            throw new ArgumentException(
                "Variant ID is required when variant name is provided.");
        }

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (unitPrice.Amount < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.");

        if (string.IsNullOrWhiteSpace(unitPrice.Currency))
            throw new ArgumentException(
                "Currency is required.");

        return new OrderLine
        {
            OrderId = orderId,
            BranchProductId = branchProductId,
            BranchProductVariantId = branchProductVariantId,
            ProductName = productName.Trim(),
            VariantName = variantName?.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = new Money(
                unitPrice.Amount * quantity,
                unitPrice.Currency)
        };
    }

    public void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Quantity = quantity;

        RecalculateTotal();
    }

    public void AddModifier(OrderLineModifier modifier)
    {
        ArgumentNullException.ThrowIfNull(modifier);

        if (modifier.OrderLineId != Id)
            throw new InvalidOperationException(
                "Modifier belongs to another order line.");

        if (_modifiers.Any(x => x.Id == modifier.Id))
            throw new InvalidOperationException(
                "Modifier already exists.");

        if (!string.Equals(
                modifier.PriceAdjustment.Currency,
                UnitPrice.Currency,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Modifier currency must match product currency.");
        }

        _modifiers.Add(modifier);

        RecalculateTotal();
    }

    public void RemoveModifier(Guid modifierId)
    {
        if (modifierId == Guid.Empty)
            throw new ArgumentException(
                "Modifier ID is required.");

        var modifier = _modifiers.FirstOrDefault(
            x => x.Id == modifierId);

        if (modifier is null)
            throw new InvalidOperationException(
                "Modifier was not found.");

        _modifiers.Remove(modifier);

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        var modifierAmount = _modifiers.Sum(
            x => x.PriceAdjustment.Amount);

        var pricePerItem =
            UnitPrice.Amount + modifierAmount;

        TotalPrice = new Money(
            pricePerItem * Quantity,
            UnitPrice.Currency);
    }
}