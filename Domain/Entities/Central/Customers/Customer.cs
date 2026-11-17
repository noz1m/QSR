using QSR.Domain.Common;

namespace QSR.Domain.Entities.Central.Customers;

public class Customer : AuditableEntity
{
    public string IdentityUserId { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string? Email { get; private set; }

    public bool IsActive { get; private set; } = true;

    public void SetIdentityUser(string identityUserId)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            throw new ArgumentException("Identity user ID is required.");

        IdentityUserId = identityUserId;
    }

    public void UpdateProfile(
        string? firstName,
        string? lastName,
        string? email)
    {
        FirstName = firstName?.Trim();
        LastName = lastName?.Trim();
        Email = email?.Trim();
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.");

        PhoneNumber = phoneNumber.Trim();
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