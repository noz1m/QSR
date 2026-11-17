
using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Recipes;

public class BranchRecipeIngredient : BaseEntity
{
    public Guid RecipeId { get; private set; }

    public Guid SourceRecipeIngredientTemplateId { get; private set; }

    public Guid BranchIngredientId { get; private set; }

    public decimal Quantity { get; private set; }

    public string UnitOfMeasureCode { get; private set; } = null!;

    private BranchRecipeIngredient()
    {
    }

    public static BranchRecipeIngredient Create(
        Guid recipeId,
        Guid sourceRecipeIngredientTemplateId,
        Guid branchIngredientId,
        decimal quantity,
        string unitOfMeasureCode)
    {
        if (recipeId == Guid.Empty)
            throw new ArgumentException("Recipe ID cannot be empty.");

        if (sourceRecipeIngredientTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Source recipe ingredient template ID cannot be empty.");

        if (branchIngredientId == Guid.Empty)
            throw new ArgumentException("Branch ingredient ID cannot be empty.");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException("Unit of measure code is required.");

        return new BranchRecipeIngredient
        {
            RecipeId = recipeId,
            SourceRecipeIngredientTemplateId = sourceRecipeIngredientTemplateId,
            BranchIngredientId = branchIngredientId,
            Quantity = quantity,
            UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant()
        };
    }

    public void UpdateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Quantity = quantity;
    }
}
