using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Menu;

public class BranchModifierGroup : AuditableEntity
{
    public Guid BranchProductId { get; private set; }

    public Guid SourceModifierGroupTemplateId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool IsRequired { get; private set; }

    public int MinSelections { get; private set; }

    public int MaxSelections { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<BranchModifierOption> _options = [];

    public IReadOnlyCollection<BranchModifierOption> Options =>
        _options.AsReadOnly();

    private BranchModifierGroup() { }

    public static BranchModifierGroup Create(
        Guid branchProductId,
        Guid sourceModifierGroupTemplateId,
        string name,
        bool isRequired,
        int minSelections,
        int maxSelections,
        int displayOrder)
    {
        if (branchProductId == Guid.Empty)
            throw new ArgumentException("Branch product ID is required.");

        if (sourceModifierGroupTemplateId == Guid.Empty)
            throw new ArgumentException("Source modifier group ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Modifier group name is required.");

        if (minSelections < 0)
            throw new ArgumentException("Minimum selections cannot be negative.");

        if (maxSelections < 1 || maxSelections < minSelections)
            throw new ArgumentException("Invalid maximum selections.");

        if (isRequired && minSelections < 1)
            throw new ArgumentException(
                "Required group must have at least one minimum selection.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        return new BranchModifierGroup
        {
            BranchProductId = branchProductId,
            SourceModifierGroupTemplateId = sourceModifierGroupTemplateId,
            Name = name.Trim(),
            IsRequired = isRequired,
            MinSelections = minSelections,
            MaxSelections = maxSelections,
            DisplayOrder = displayOrder
        };
    }

    public void AddOption(BranchModifierOption option)
    {
        ArgumentNullException.ThrowIfNull(option);

        if (option.BranchModifierGroupId != Id)
            throw new InvalidOperationException(
                "Option belongs to another modifier group.");

        if (_options.Any(x => x.Id == option.Id))
            throw new InvalidOperationException("Option already exists.");

        _options.Add(option);
    }

    public void UpdateDetails(
        string name,
        bool isRequired,
        int minSelections,
        int maxSelections,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Modifier group name is required.");

        if (minSelections < 0)
            throw new ArgumentException("Minimum selections cannot be negative.");

        if (maxSelections < 1 || maxSelections < minSelections)
            throw new ArgumentException("Invalid maximum selections.");

        if (isRequired && minSelections < 1)
            throw new ArgumentException(
                "Required group must have at least one minimum selection.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        Name = name.Trim();
        IsRequired = isRequired;
        MinSelections = minSelections;
        MaxSelections = maxSelections;
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}