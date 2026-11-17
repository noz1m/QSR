using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Inventory;

public class Ingredient : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public decimal CurrentStock { get; private set; }

    public decimal MinimumStock { get; private set; }

    public string UnitOfMeasureCode { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    public bool IsLowStock =>
        CurrentStock <= MinimumStock;

    private Ingredient()
    {
    }

    public static Ingredient Create(
        string name,
        string unitOfMeasureCode,
        decimal minimumStock,
        decimal initialStock = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Ingredient name is required.");

        if (string.IsNullOrWhiteSpace(unitOfMeasureCode))
            throw new ArgumentException(
                "Unit of measure is required.");

        if (minimumStock < 0)
            throw new ArgumentException(
                "Minimum stock cannot be negative.");

        if (initialStock < 0)
            throw new ArgumentException(
                "Initial stock cannot be negative.");

        return new Ingredient
        {
            Name = name.Trim(),
            UnitOfMeasureCode =
                unitOfMeasureCode.Trim().ToUpperInvariant(),
            MinimumStock = minimumStock,
            CurrentStock = initialStock
        };
    }

    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        CurrentStock += quantity;
    }

    public void DeductStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (CurrentStock < quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for ingredient {Name}. " +
                $"Available: {CurrentStock} {UnitOfMeasureCode}, " +
                $"required: {quantity} {UnitOfMeasureCode}.");
        }

        CurrentStock -= quantity;
    }

    public void UpdateMinimumStock(
        decimal minimumStock)
    {
        if (minimumStock < 0)
            throw new ArgumentException(
                "Minimum stock cannot be negative.");

        MinimumStock = minimumStock;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Ingredient name is required.");

        Name = name.Trim();
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