using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Menu;

public class BranchMenu : AuditableEntity
{
    public Guid BranchId { get; private set; }

    public Guid SourceMenuTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int SourceVersion { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<BranchCategory> _categories = [];

    public IReadOnlyCollection<BranchCategory> Categories =>
        _categories.AsReadOnly();

    private BranchMenu() { }

    public static BranchMenu Create(
        Guid branchId,
        Guid sourceMenuTemplateId,
        string name,
        int sourceVersion)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException("Branch ID is required.");

        if (sourceMenuTemplateId == Guid.Empty)
            throw new ArgumentException("Source menu template ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Menu name is required.");

        if (sourceVersion < 1)
            throw new ArgumentException("Source version must be greater than zero.");

        return new BranchMenu
        {
            BranchId = branchId,
            SourceMenuTemplateId = sourceMenuTemplateId,
            Name = name.Trim(),
            SourceVersion = sourceVersion
        };
    }

    public void AddCategory(BranchCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (category.BranchMenuId != Id)
            throw new InvalidOperationException(
                "Category belongs to another branch menu.");

        if (_categories.Any(x => x.Id == category.Id))
            throw new InvalidOperationException("Category already exists.");

        _categories.Add(category);
    }

    public void UpdateSourceVersion(int version)
    {
        if (version <= SourceVersion)
            throw new ArgumentException(
                "New source version must be greater than current version.");

        SourceVersion = version;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}