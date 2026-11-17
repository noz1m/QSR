using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Menu;

public class BranchCategory : AuditableEntity
{
    public Guid BranchMenuId { get; private set; }

    public Guid SourceCategoryTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<BranchProduct> _products = [];

    public IReadOnlyCollection<BranchProduct> Products =>
        _products.AsReadOnly();

    private BranchCategory() { }

    public static BranchCategory Create(
        Guid branchMenuId,
        Guid sourceCategoryTemplateId,
        string name,
        int displayOrder)
    {
        if (branchMenuId == Guid.Empty)
            throw new ArgumentException("Branch menu ID is required.");

        if (sourceCategoryTemplateId == Guid.Empty)
            throw new ArgumentException("Source category ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        return new BranchCategory
        {
            BranchMenuId = branchMenuId,
            SourceCategoryTemplateId = sourceCategoryTemplateId,
            Name = name.Trim(),
            DisplayOrder = displayOrder
        };
    }

    public void AddProduct(BranchProduct product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (product.BranchCategoryId != Id)
            throw new InvalidOperationException(
                "Product belongs to another category.");

        if (_products.Any(x => x.Id == product.Id))
            throw new InvalidOperationException("Product already exists.");

        _products.Add(product);
    }

    public void UpdateDetails(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        Name = name.Trim();
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}