using QSR.Domain.Common;
using QSR.Domain.ValueObjects;
using QSR.Domain.Entities.Branches.Recipes;

namespace QSR.Domain.Entities.Branches.Menu;

public class BranchProduct : AuditableEntity
{
    public Guid BranchCategoryId { get; private set; }

    public Guid SourceProductTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? ImageUrl { get; private set; }

    public Money CentralPrice { get; private set; }

    public Money LocalPrice { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsAvailable { get; private set; } = true;

    public string? UnavailableReason { get; private set; }

    public bool IsActive { get; private set; } = true;
    public Guid? KitchenStationId { get; private set; }

    public int PreparationTimeMinutes { get; private set; }

    private readonly List<BranchProductVariant> _variants = [];

    public IReadOnlyCollection<BranchProductVariant> Variants =>
        _variants.AsReadOnly();

    private readonly List<BranchModifierGroup> _modifierGroups = [];

    public IReadOnlyCollection<BranchModifierGroup> ModifierGroups =>
        _modifierGroups.AsReadOnly();

    private BranchProduct() { }

    public void UpdateKitchenSettings(
    Guid kitchenStationId,
    int preparationTimeMinutes)
    {
        if (kitchenStationId == Guid.Empty)
            throw new ArgumentException(
                "Kitchen station ID is required.");

        if (preparationTimeMinutes <= 0)
            throw new ArgumentException(
                "Preparation time must be greater than zero.");

        KitchenStationId = kitchenStationId;
        PreparationTimeMinutes = preparationTimeMinutes;
    }

    public static BranchProduct Create(
        Guid branchCategoryId,
        Guid sourceProductTemplateId,
        string name,
        Money centralPrice,
        int displayOrder,
        string? description = null,
        string? imageUrl = null)
    {
        if (branchCategoryId == Guid.Empty)
            throw new ArgumentException("Branch category ID is required.");

        if (sourceProductTemplateId == Guid.Empty)
            throw new ArgumentException("Source product ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (centralPrice.Amount < 0)
            throw new ArgumentException("Product price cannot be negative.");

        if (string.IsNullOrWhiteSpace(centralPrice.Currency))
            throw new ArgumentException("Currency is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        return new BranchProduct
        {
            BranchCategoryId = branchCategoryId,
            SourceProductTemplateId = sourceProductTemplateId,
            Name = name.Trim(),
            Description = description?.Trim(),
            ImageUrl = imageUrl?.Trim(),
            CentralPrice = centralPrice,
            LocalPrice = centralPrice,
            DisplayOrder = displayOrder
        };
    }

    public void AddVariant(BranchProductVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        if (variant.BranchProductId != Id)
            throw new InvalidOperationException(
                "Variant belongs to another product.");

        if (_variants.Any(x => x.Id == variant.Id))
            throw new InvalidOperationException("Variant already exists.");

        if (!string.Equals(
                variant.LocalPrice.Currency,
                LocalPrice.Currency,
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "Variant and product currencies must match.");

        _variants.Add(variant);
    }

    public void AddModifierGroup(BranchModifierGroup modifierGroup)
    {
        ArgumentNullException.ThrowIfNull(modifierGroup);

        if (modifierGroup.BranchProductId != Id)
            throw new InvalidOperationException(
                "Modifier group belongs to another product.");

        if (_modifierGroups.Any(x => x.Id == modifierGroup.Id))
            throw new InvalidOperationException(
                "Modifier group already exists.");

        foreach (var option in modifierGroup.Options)
        {
            if (!string.Equals(
                    option.LocalPriceAdjustment.Currency,
                    LocalPrice.Currency,
                    StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "Modifier and product currencies must match.");
        }

        _modifierGroups.Add(modifierGroup);
    }

    public void UpdateLocalPrice(Money price)
    {
        if (price.Amount < 0)
            throw new ArgumentException("Product price cannot be negative.");

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

    public void SetAvailability(bool isAvailable, string? reason = null)
    {
        IsAvailable = isAvailable;
        UnavailableReason = isAvailable ? null : reason?.Trim();
    }

    public void UpdateDetails(
        string name,
        string? description,
        string? imageUrl,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        Name = name.Trim();
        Description = description?.Trim();
        ImageUrl = imageUrl?.Trim();
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private readonly List<BranchRecipe> _recipes = [];

    public IReadOnlyCollection<BranchRecipe> Recipes => _recipes.AsReadOnly();

    public void AddRecipe(BranchRecipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (recipe.BranchProductId != Id)
            throw new InvalidOperationException(
                "Recipe belongs to another product.");

        if (_recipes.Any(x => x.Id == recipe.Id))
            throw new InvalidOperationException(
                "Recipe already exists for this product.");

        _recipes.Add(recipe);
    }
}