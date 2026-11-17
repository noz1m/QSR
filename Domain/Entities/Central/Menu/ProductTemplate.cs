using QSR.Domain.Common;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Central.Menu;

public class ProductTemplate : AuditableEntity
{
    public Guid CategoryTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? ImageUrl { get; private set; }

    public Money BasePrice { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<ProductVariantTemplate> _variants = [];

    public IReadOnlyCollection<ProductVariantTemplate> Variants =>
        _variants.AsReadOnly();

    private readonly List<ModifierGroupTemplate> _modifierGroups = [];

    public IReadOnlyCollection<ModifierGroupTemplate> ModifierGroups =>
        _modifierGroups.AsReadOnly();

    private ProductTemplate()
    {
    }

    public static ProductTemplate Create(
        Guid categoryTemplateId,
        string name,
        Money basePrice,
        string? description = null,
        string? imageUrl = null)
    {
        if (categoryTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Category ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Product name is required.");

        if (basePrice.Amount < 0)
            throw new ArgumentException(
                "Product price cannot be negative.");

        if (string.IsNullOrWhiteSpace(basePrice.Currency))
            throw new ArgumentException(
                "Currency is required.");

        return new ProductTemplate
        {
            CategoryTemplateId = categoryTemplateId,
            Name = name.Trim(),
            Description = description?.Trim(),
            ImageUrl = imageUrl?.Trim(),
            BasePrice = basePrice
        };
    }

    public void AddVariant(
        ProductVariantTemplate variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        if (variant.ProductTemplateId != Id)
        {
            throw new InvalidOperationException(
                "Variant belongs to another product.");
        }

        if (_variants.Any(x => x.Id == variant.Id))
        {
            throw new InvalidOperationException(
                "Variant already exists.");
        }

        if (!string.Equals(
                variant.Price.Currency,
                BasePrice.Currency,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Product and variant currencies must match.");
        }

        _variants.Add(variant);
    }

    public void AddModifierGroup(
        ModifierGroupTemplate modifierGroup)
    {
        ArgumentNullException.ThrowIfNull(modifierGroup);

        if (modifierGroup.ProductTemplateId != Id)
        {
            throw new InvalidOperationException(
                "Modifier group belongs to another product.");
        }

        if (_modifierGroups.Any(x => x.Id == modifierGroup.Id))
        {
            throw new InvalidOperationException(
                "Modifier group already exists.");
        }

        _modifierGroups.Add(modifierGroup);
    }

    public void UpdateDetails(
        string name,
        string? description,
        string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Product name is required.");

        Name = name.Trim();
        Description = description?.Trim();
        ImageUrl = imageUrl?.Trim();
    }

    public void UpdateBasePrice(
        Money price)
    {
        if (price.Amount < 0)
            throw new ArgumentException(
                "Product price cannot be negative.");

        if (string.IsNullOrWhiteSpace(price.Currency))
            throw new ArgumentException(
                "Currency is required.");

        if (_variants.Any(x =>
                !string.Equals(
                    x.Price.Currency,
                    price.Currency,
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Product and variant currencies must match.");
        }

        BasePrice = price;
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