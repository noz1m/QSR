using QSR.Domain.Common;

namespace QSR.Domain.Entities.Central.Recipes;

public class IngredientTemplate : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string UnitOfMeasureCode { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    private IngredientTemplate() { }

    public static IngredientTemplate Create(
        string name,
        string code,
        string unitOfMeasureCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ingredient name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Ingredient code is required.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException("Unit of measure is required.");

        return new IngredientTemplate
        {
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant(),
            UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant()
        };
    }

    public void UpdateDetails(
        string name,
        string code,
        string unitOfMeasureCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ingredient name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Ingredient code is required.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException("Unit of measure is required.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        UnitOfMeasureCode = unitOfMeasureCode.Trim().ToUpperInvariant();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}