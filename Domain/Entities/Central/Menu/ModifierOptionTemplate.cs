using QSR.Domain.Common;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Central.Menu;

public class ModifierOptionTemplate : AuditableEntity
{
    public Guid ModifierGroupTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Money PriceAdjustment { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private ModifierOptionTemplate()
    {
    }

    public static ModifierOptionTemplate Create(
        Guid modifierGroupTemplateId,
        string name,
        Money priceAdjustment,
        int displayOrder,
        string? description = null)
    {
        if (modifierGroupTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Modifier group ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Modifier option name is required.");

        if (priceAdjustment.Amount < 0)
            throw new ArgumentException(
                "Price adjustment cannot be negative.");

        if (string.IsNullOrWhiteSpace(priceAdjustment.Currency))
            throw new ArgumentException(
                "Currency is required.");

        if (displayOrder < 0)
            throw new ArgumentException(
                "Display order cannot be negative.");

        return new ModifierOptionTemplate
        {
            ModifierGroupTemplateId = modifierGroupTemplateId,
            Name = name.Trim(),
            Description = description?.Trim(),
            PriceAdjustment = priceAdjustment,
            DisplayOrder = displayOrder
        };
    }

    public void UpdateDetails(
        string name,
        string? description,
        Money priceAdjustment,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Modifier option name is required.");

        if (priceAdjustment.Amount < 0)
            throw new ArgumentException(
                "Price adjustment cannot be negative.");

        if (string.IsNullOrWhiteSpace(priceAdjustment.Currency))
            throw new ArgumentException(
                "Currency is required.");

        if (displayOrder < 0)
            throw new ArgumentException(
                "Display order cannot be negative.");

        Name = name.Trim();
        Description = description?.Trim();
        PriceAdjustment = priceAdjustment;
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