using QSR.Domain.Common;
using QSR.Domain.Enums;

namespace QSR.Domain.Entities.Branches.Menu;

public class BranchModifierOptionIngredient : BaseEntity
{
    public Guid BranchModifierOptionId { get; private set; }

    public Guid SourceModifierOptionIngredientTemplateId { get; private set; }

    public Guid BranchIngredientId { get; private set; }

    public decimal Quantity { get; private set; }

    public string UnitOfMeasureCode { get; private set; } = null!;

    public IngredientChangeType ChangeType { get; private set; }

    private BranchModifierOptionIngredient()
    {
    }

    private readonly List<BranchModifierOptionIngredient> _ingredientChanges = [];

    public IReadOnlyCollection<BranchModifierOptionIngredient> IngredientChanges
        => _ingredientChanges.AsReadOnly();

    public static BranchModifierOptionIngredient Create(
        Guid branchModifierOptionId,
        Guid sourceModifierOptionIngredientTemplateId,
        Guid branchIngredientId,
        decimal quantity,
        string unitOfMeasureCode,
        IngredientChangeType changeType)
    {
        if (branchModifierOptionId == Guid.Empty)
            throw new ArgumentException(
                "Branch modifier option ID cannot be empty.");

        if (sourceModifierOptionIngredientTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Source modifier option ingredient template ID cannot be empty.");

        if (branchIngredientId == Guid.Empty)
            throw new ArgumentException(
                "Branch ingredient ID cannot be empty.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException(
                "Unit of measure code is required.");

        if (!Enum.IsDefined(changeType))
            throw new ArgumentException(
                "Invalid ingredient change type.");

        return new BranchModifierOptionIngredient
        {
            BranchModifierOptionId = branchModifierOptionId,
            SourceModifierOptionIngredientTemplateId =
                sourceModifierOptionIngredientTemplateId,
            BranchIngredientId = branchIngredientId,
            Quantity = quantity,
            UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant(),
            ChangeType = changeType
        };
    }

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

    public void UpdateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Quantity = quantity;
    }
}