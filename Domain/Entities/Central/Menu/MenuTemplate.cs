using QSR.Domain.Common;

namespace QSR.Domain.Entities.Central.Menu;

public class MenuTemplate : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<CategoryTemplate> _categories = [];

    public IReadOnlyCollection<CategoryTemplate> Categories => _categories.AsReadOnly();

    private MenuTemplate()
    {
    }

    public static MenuTemplate Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Menu name is required.");

        return new MenuTemplate
        {
            Name = name.Trim(),
            Description = description?.Trim()
        };
    }

    public void AddCategory(CategoryTemplate category)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (_categories.Any(x => x.Id == category.Id))
            throw new InvalidOperationException("Category already exists.");

        _categories.Add(category);
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