using QSR.Domain.Common;
using QSR.Domain.Enums;

namespace QSR.Domain.Entities.Branches.Kitchen;

public class KitchenStation : AuditableEntity
{
    public Guid BranchId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public KitchenStationType Type { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private KitchenStation() { }

    public static KitchenStation Create(
        Guid branchId,
        string name,
        string code,
        KitchenStationType type,
        int displayOrder)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException("Branch ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Station name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Station code is required.");

        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid kitchen station type.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        return new KitchenStation
        {
            BranchId = branchId,
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant(),
            Type = type,
            DisplayOrder = displayOrder
        };
    }

    public void UpdateDetails(
        string name,
        string code,
        KitchenStationType type,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Station name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Station code is required.");

        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid kitchen station type.");

        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Type = type;
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}