using QSR.Domain.Common;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Branches.Menu;

public class BranchProductVariant : AuditableEntity
{
    public Guid BranchProductId { get; private set; }

    public Guid SourceProductVariantTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Money CentralPrice { get; private set; }

    public Money LocalPrice { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private BranchProductVariant() { }

    public static BranchProductVariant Create(
        Guid branchProductId,
        Guid sourceProductVariantTemplateId,
        string name,
        Money centralPrice,
        int displayOrder)
    {
        if (branchProductId == Guid.Empty)
            throw new ArgumentException("Branch product ID is required.");

        if (sourceProductVariantTemplateId == Guid.Empty)
            throw new ArgumentException("Source variant ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Variant name is required.");

        if (centralPrice.Amount < 0)
            throw new ArgumentException("Variant price cannot be negative.");

        if (string.IsNullOrWhiteSpace(centralPrice.Currency))
            throw new ArgumentException("Currency is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        return new BranchProductVariant
        {
            BranchProductId = branchProductId,
            SourceProductVariantTemplateId = sourceProductVariantTemplateId,
            Name = name.Trim(),
            CentralPrice = centralPrice,
            LocalPrice = centralPrice,
            DisplayOrder = displayOrder
        };
    }

    public void UpdateLocalPrice(Money price)
    {
        if (price.Amount < 0)
            throw new ArgumentException("Variant price cannot be negative.");

        if (!string.Equals(
                price.Currency,
                CentralPrice.Currency,
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "Local and central prices must use the same currency.");

        LocalPrice = price;
    }

    public void ResetLocalPrice()
    {
        LocalPrice = CentralPrice;
    }

    public void UpdateDetails(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Variant name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        Name = name.Trim();
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}