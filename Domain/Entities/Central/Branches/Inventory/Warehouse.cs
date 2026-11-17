using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Inventory;

public class Warehouse : AuditableEntity
{
    public Guid BranchId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    private Warehouse() { }

    public static Warehouse Create(
        Guid branchId,
        string name,
        string code,
        string? description = null)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException("Branch ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Warehouse code is required.");

        return new Warehouse
        {
            BranchId = branchId,
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant(),
            Description = description?.Trim()
        };
    }

    public void UpdateDetails(
        string name,
        string code,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Warehouse code is required.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}