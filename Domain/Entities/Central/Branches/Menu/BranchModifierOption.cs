using QSR.Domain.Common;
using QSR.Domain.ValueObjects;
using QSR.Domain.Entities.Branches.Inventory;

namespace QSR.Domain.Entities.Branches.Menu;

public class BranchModifierOption : AuditableEntity
{
    public Guid BranchModifierGroupId { get; private set; }

    public Guid SourceModifierOptionTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Money CentralPriceAdjustment { get; private set; }

    public Money LocalPriceAdjustment { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private BranchModifierOption() { }


    private readonly List<BranchModifierOptionIngredient> _ingredientChanges = [];

    public IReadOnlyCollection<BranchModifierOptionIngredient> IngredientChanges
        => _ingredientChanges.AsReadOnly();

    public void AddIngredientChange(
        BranchModifierOptionIngredient ingredientChange)
    {
        ArgumentNullException.ThrowIfNull(ingredientChange);

        if (ingredientChange.BranchModifierOptionId != Id)
            throw new InvalidOperationException(
                "Ingredient change belongs to another modifier option.");

        if (_ingredientChanges.Any(x =>
                x.BranchIngredientId == ingredientChange.BranchIngredientId))
            throw new InvalidOperationException(
                "Ingredient change already exists for this modifier option.");

        _ingredientChanges.Add(ingredientChange);
    }
    public static BranchModifierOption Create(
        Guid branchModifierGroupId,
        Guid sourceModifierOptionTemplateId,
        string name,
        Money centralPriceAdjustment,
        int displayOrder,
        string? description = null)
    {
        if (branchModifierGroupId == Guid.Empty)
            throw new ArgumentException("Modifier group ID is required.");

        if (sourceModifierOptionTemplateId == Guid.Empty)
            throw new ArgumentException("Source modifier option ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Modifier option name is required.");

        if (centralPriceAdjustment.Amount < 0)
            throw new ArgumentException(
                "Price adjustment cannot be negative.");

        if (string.IsNullOrWhiteSpace(centralPriceAdjustment.Currency))
            throw new ArgumentException("Currency is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        return new BranchModifierOption
        {
            BranchModifierGroupId = branchModifierGroupId,
            SourceModifierOptionTemplateId = sourceModifierOptionTemplateId,
            Name = name.Trim(),
            Description = description?.Trim(),
            CentralPriceAdjustment = centralPriceAdjustment,
            LocalPriceAdjustment = centralPriceAdjustment,
            DisplayOrder = displayOrder
        };
    }

    public void UpdateLocalPriceAdjustment(Money priceAdjustment)
    {
        if (priceAdjustment.Amount < 0)
            throw new ArgumentException(
                "Price adjustment cannot be negative.");

        if (!string.Equals(
                priceAdjustment.Currency,
                CentralPriceAdjustment.Currency,
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "Local and central currencies must match.");

        LocalPriceAdjustment = priceAdjustment;
    }

    public void ResetLocalPriceAdjustment()
    {
        LocalPriceAdjustment = CentralPriceAdjustment;
    }

    public void UpdateDetails(
        string name,
        string? description,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Modifier option name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        Name = name.Trim();
        Description = description?.Trim();
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}