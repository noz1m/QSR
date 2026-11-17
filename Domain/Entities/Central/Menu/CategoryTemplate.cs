using QSR.Domain.Common;
using ProductTemplateEntity = QSR.Domain.Entities.Central.Menu.ProductTemplate;

namespace QSR.Domain.Entities.Central.Menu;

public class CategoryTemplate : AuditableEntity
{
    public Guid MenuTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<ProductTemplateEntity> _products = [];

    public IReadOnlyCollection<ProductTemplateEntity> Products =>
        _products.AsReadOnly();

    private CategoryTemplate()
    {
    }

    public static CategoryTemplate Create(
        Guid menuTemplateId,
        string name,
        string? description,
        int displayOrder)
    {
        if (menuTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Menu template ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Category name is required.");

        if (displayOrder < 0)
            throw new ArgumentException(
                "Display order cannot be negative.");

        return new CategoryTemplate
        {
            MenuTemplateId = menuTemplateId,
            Name = name.Trim(),
            Description = description?.Trim(),
            DisplayOrder = displayOrder
        };
    }

    public void AddProduct(
        ProductTemplateEntity product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (product.CategoryTemplateId != Id)
        {
            throw new InvalidOperationException(
                "Product belongs to another category.");
        }

        if (_products.Any(x => x.Id == product.Id))
        {
            throw new InvalidOperationException(
                "Product already exists.");
        }

        _products.Add(product);
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