
using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Recipes;

public class BranchRecipe : AuditableEntity
{
    private readonly List<BranchRecipeIngredient> _ingredients = [];

    public Guid BranchProductId { get; private set; }

    public Guid SourceRecipeTemplateId { get; private set; }

    public int Version { get; private set; }

    public DateTimeOffset EffectiveFrom { get; private set; }

    public DateTimeOffset? EffectiveTo { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<BranchRecipeIngredient> Ingredients => _ingredients.AsReadOnly();

    private BranchRecipe()
    {
    }

    public static BranchRecipe Create(
        Guid branchProductId,
        Guid sourceRecipeTemplateId,
        int version,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo = null)
    {
        if (branchProductId == Guid.Empty)
            throw new ArgumentException("Branch product ID cannot be empty.");

        if (sourceRecipeTemplateId == Guid.Empty)
            throw new ArgumentException("Source recipe template ID cannot be empty.");

        if (version < 1)
            throw new ArgumentException("Recipe version must be greater than zero.");

        if (effectiveTo.HasValue && effectiveTo <= effectiveFrom)
            throw new ArgumentException("Effective end date must be after start date.");

        return new BranchRecipe
        {
            BranchProductId = branchProductId,
            SourceRecipeTemplateId = sourceRecipeTemplateId,
            Version = version,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            IsActive = true
        };
    }

    public void AddIngredient(BranchRecipeIngredient ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient);

        if (ingredient.RecipeId != Id)
            throw new InvalidOperationException(
                "Ingredient belongs to another recipe.");

        if (_ingredients.Any(x => x.BranchIngredientId == ingredient.BranchIngredientId))
            throw new InvalidOperationException(
                "Ingredient already exists in this recipe.");

        _ingredients.Add(ingredient);
    }

    public void SetEffectivePeriod(
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo)
    {
        if (effectiveTo.HasValue && effectiveTo <= effectiveFrom)
            throw new ArgumentException("Effective end date must be after start date.");

        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
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
