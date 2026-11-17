using QSR.Domain.Common;

namespace QSR.Domain.Entities.Central.Recipes;

public class RecipeIngredientTemplate : AuditableEntity
{
    public Guid RecipeTemplateId { get; private set; }

    public Guid IngredientTemplateId { get; private set; }

    public decimal Quantity { get; private set; }

    public string UnitOfMeasureCode { get; private set; } = string.Empty;

    private RecipeIngredientTemplate() { }

    public static RecipeIngredientTemplate Create(
        Guid recipeTemplateId,
        Guid ingredientTemplateId,
        decimal quantity,
        string unitOfMeasureCode)
    {
        if (recipeTemplateId == Guid.Empty)
            throw new ArgumentException("Recipe ID is required.");

        if (ingredientTemplateId == Guid.Empty)
            throw new ArgumentException("Ingredient ID is required.");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException("Unit of measure is required.");

        return new RecipeIngredientTemplate
        {
            RecipeTemplateId = recipeTemplateId,
            IngredientTemplateId = ingredientTemplateId,
            Quantity = quantity,
            UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant()
        };
    }

    public void UpdateQuantity(
        decimal quantity,
        string unitOfMeasureCode)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException("Unit of measure is required.");

        Quantity = quantity;
        UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant();
    }
}