using QSR.Domain.Common;

namespace QSR.Domain.Entities.Central.Menu;

public class ModifierGroupTemplate : AuditableEntity
{
    public Guid ProductTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool IsRequired { get; private set; }

    public int MinSelections { get; private set; }

    public int MaxSelections { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<ModifierOptionTemplate> _options = [];

    public IReadOnlyCollection<ModifierOptionTemplate> Options =>
        _options.AsReadOnly();

    private ModifierGroupTemplate()
    {
    }

    public static ModifierGroupTemplate Create(
        Guid productTemplateId,
        string name,
        bool isRequired,
        int minSelections,
        int maxSelections)
    {
        if (productTemplateId == Guid.Empty)
            throw new ArgumentException(
                "Product template ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Modifier group name is required.");

        if (minSelections < 0)
            throw new ArgumentException(
                "Minimum selections cannot be negative.");

        if (maxSelections < 1)
            throw new ArgumentException(
                "Maximum selections must be greater than zero.");

        if (maxSelections < minSelections)
            throw new ArgumentException(
                "Maximum selections cannot be less than minimum selections.");

        if (isRequired && minSelections == 0)
            throw new ArgumentException(
                "Required modifier group must require at least one selection.");

        return new ModifierGroupTemplate
        {
            ProductTemplateId = productTemplateId,
            Name = name.Trim(),
            IsRequired = isRequired,
            MinSelections = minSelections,
            MaxSelections = maxSelections
        };
    }

    public void AddOption(
        ModifierOptionTemplate option)
    {
        ArgumentNullException.ThrowIfNull(option);

        if (option.ModifierGroupTemplateId != Id)
            throw new InvalidOperationException(
                "Modifier option belongs to another group.");

        if (_options.Any(x => x.Id == option.Id))
            throw new InvalidOperationException(
                "Modifier option already exists.");

        _options.Add(option);
    }

    public void Update(
        string name,
        bool isRequired,
        int minSelections,
        int maxSelections)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Modifier group name is required.");

        if (minSelections < 0)
            throw new ArgumentException(
                "Minimum selections cannot be negative.");

        if (maxSelections < 1)
            throw new ArgumentException(
                "Maximum selections must be greater than zero.");

        if (maxSelections < minSelections)
            throw new ArgumentException(
                "Maximum selections cannot be less than minimum selections.");

        Name = name.Trim();
        IsRequired = isRequired;
        MinSelections = minSelections;
        MaxSelections = maxSelections;
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