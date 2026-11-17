using QSR.Domain.Common;
using QSR.Domain.Enums;
using QSR.Domain.Entities.Central.Recipes;

namespace QSR.Domain.Entities.Central.Recipes;

public class ModifierOptionIngredientTemplate : BaseEntity
{
    public Guid ModifierOptionTemplateId { get; private set; }

    public Guid IngredientTemplateId { get; private set; }

    public decimal Quantity { get; private set; }

    public string UnitOfMeasureCode { get; private set; } = null!;

    public IngredientChangeType ChangeType { get; private set; }

    private ModifierOptionIngredientTemplate()
    {
    }
    private readonly List<ModifierOptionIngredientTemplate> _ingredientChanges = [];

    public IReadOnlyCollection<ModifierOptionIngredientTemplate> IngredientChanges
        => _ingredientChanges.AsReadOnly();

    public void AddIngredientChange(
    ModifierOptionIngredientTemplate ingredientChange)
    {
        ArgumentNullException.ThrowIfNull(ingredientChange);

        if (ingredientChange.ModifierOptionTemplateId != Id)
            throw new InvalidOperationException(
                "Ingredient change belongs to another modifier option.");

        if (_ingredientChanges.Any(x =>
                x.IngredientTemplateId == ingredientChange.IngredientTemplateId))
            throw new InvalidOperationException(
                "Ingredient change already exists for this modifier option.");

        _ingredientChanges.Add(ingredientChange);
    }

    public static ModifierOptionIngredientTemplate Create(
        Guid modifierOptionTemplateId,
        Guid ingredientTemplateId,
        decimal quantity,
        string unitOfMeasureCode,
        IngredientChangeType changeType)
    {
        if (modifierOptionTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Modifier option template ID cannot be empty.");

        if (ingredientTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Ingredient template ID cannot be empty.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException(
                "Unit of measure code is required.");

        if (!Enum.IsDefined(changeType))
            throw new ArgumentException(
                "Invalid ingredient change type.");

        return new ModifierOptionIngredientTemplate
        {
            ModifierOptionTemplateId = modifierOptionTemplateId,
            IngredientTemplateId = ingredientTemplateId,
            Quantity = quantity,
            UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant(),
            ChangeType = changeType
        };
    }
}