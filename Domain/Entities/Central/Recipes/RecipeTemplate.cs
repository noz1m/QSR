using QSR.Domain.Common;

namespace QSR.Domain.Entities.Central.Recipes;

public class RecipeTemplate : AuditableEntity
{
    public Guid ProductTemplateId { get; private set; }

    public int Version { get; private set; }

    public DateTimeOffset EffectiveFrom { get; private set; }

    public DateTimeOffset? EffectiveTo { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<RecipeIngredientTemplate> _ingredients = [];

    public IReadOnlyCollection<RecipeIngredientTemplate> Ingredients =>
        _ingredients.AsReadOnly();

    private RecipeTemplate() { }

    public static RecipeTemplate Create(
        Guid productTemplateId,
        int version,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo = null)
    {
        if (productTemplateId == Guid.Empty)
            throw new ArgumentException("Product template ID is required.");

        if (version < 1)
            throw new ArgumentException("Recipe version must be greater than zero.");

        if (effectiveTo.HasValue && effectiveTo.Value <= effectiveFrom)
            throw new ArgumentException("Effective end date must be after start date.");

        return new RecipeTemplate
        {
            ProductTemplateId = productTemplateId,
            Version = version,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo
        };
    }

    public void AddIngredient(RecipeIngredientTemplate ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient);

        if (ingredient.RecipeTemplateId != Id)
            throw new InvalidOperationException(
                "Ingredient belongs to another recipe.");

        if (_ingredients.Any(x =>
                x.IngredientTemplateId == ingredient.IngredientTemplateId))
            throw new InvalidOperationException(
                "Ingredient already exists in this recipe.");

        _ingredients.Add(ingredient);
    }

    public void SetEffectivePeriod(
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo)
    {
        if (effectiveTo.HasValue && effectiveTo.Value <= effectiveFrom)
            throw new ArgumentException(
                "Effective end date must be after start date.");

        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}