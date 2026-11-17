using QSR.Domain.Common;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Central.Menu;

public class ProductVariantTemplate : AuditableEntity
{
    public Guid ProductTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Money Price { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private ProductVariantTemplate()
    {
    }

    public static ProductVariantTemplate Create(
        Guid productTemplateId,
        string name,
        Money price,
        int displayOrder)
    {
        if (productTemplateId == Guid.Empty)
            throw new ArgumentException("Product ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Variant name is required.");

        if (price.Amount < 0)
            throw new ArgumentException("Variant price cannot be negative.");

        if (string.IsNullOrWhiteSpace(price.Currency))
            throw new ArgumentException("Currency is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        return new ProductVariantTemplate
        {
            ProductTemplateId = productTemplateId,
            Name = name.Trim(),
            Price = price,
            DisplayOrder = displayOrder
        };
    }

    public void UpdateDetails(string name, Money price, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Variant name is required.");

        if (price.Amount < 0)
            throw new ArgumentException("Variant price cannot be negative.");

        if (string.IsNullOrWhiteSpace(price.Currency))
            throw new ArgumentException("Currency is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        Name = name.Trim();
        Price = price;
        DisplayOrder = displayOrder;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}