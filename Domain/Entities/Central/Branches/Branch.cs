using QSR.Domain.Common;

namespace QSR.Domain.Entities.Central.Branches;

public class Branch : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public string? PhoneNumber { get; private set; }

    public bool IsActive { get; private set; } = true;

    public void UpdateDetails(
        string name,
        string code,
        string address,
        string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Branch name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Branch code is required.");

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Branch address is required.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Address = address.Trim();
        PhoneNumber = phoneNumber?.Trim();
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