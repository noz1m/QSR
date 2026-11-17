using QSR.Domain.Common;

namespace QSR.Domain.Entities.Branches.Cash;

public class CashShift : AuditableEntity
{
    private readonly List<CashEntry> _entries = [];
    public Guid BranchId { get; private set; }
    public string RegisterCode { get; private set; } = "";
    public string Currency { get; private set; } = "";
    public string OpenedBy { get; private set; } = "";
    public decimal OpeningAmount { get; private set; }
    public decimal ExpectedAmount { get; private set; }
    public decimal? CountedAmount { get; private set; }
    public decimal? Difference => CountedAmount - ExpectedAmount;
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }
    public IReadOnlyCollection<CashEntry> Entries => _entries.AsReadOnly();

    private CashShift() { }

    public static CashShift Open(Guid branchId, string registerCode,
        string currency, decimal openingAmount, string actor)
    {
        if (branchId == Guid.Empty || string.IsNullOrWhiteSpace(registerCode) ||
            registerCode.Length > 50 || string.IsNullOrWhiteSpace(actor) || actor.Length > 200)
            throw new ArgumentException("Branch, register and actor are required.");
        if (currency is not { Length: 3 } || !currency.All(char.IsAsciiLetter))
            throw new ArgumentException("Three-letter currency is required.");
        ValidateAmount(openingAmount);

        return new CashShift
        {
            BranchId = branchId,
            RegisterCode = registerCode.Trim().ToUpperInvariant(),
            Currency = currency.ToUpperInvariant(),
            OpeningAmount = openingAmount,
            ExpectedAmount = openingAmount,
            OpenedBy = actor
        };
    }

    public void Record(Guid key, CashEntryType type, decimal amount,
        string actor, string reason, Guid? paymentId = null)
    {
        if (key == Guid.Empty || !Enum.IsDefined(type) || amount <= 0 ||
            string.IsNullOrWhiteSpace(actor) || actor.Length > 200 ||
            string.IsNullOrWhiteSpace(reason) || reason.Length > 500)
            throw new ArgumentException("Valid entry details are required.");
        ValidateAmount(amount);
        if ((type is CashEntryType.Sale or CashEntryType.Refund) != paymentId.HasValue)
            throw new ArgumentException("Sales and refunds must reference a payment.");

        var existing = _entries.SingleOrDefault(x => x.RequestKey == key);
        if (existing is not null)
        {
            if (existing.Type != type || existing.Amount != amount ||
                existing.PaymentId != paymentId || existing.Reason != reason ||
                existing.Actor != actor)
                throw new CashShiftConflictException("Entry key was used with different data.");
            return;
        }
        if (ClosedAt.HasValue)
            throw new CashShiftConflictException("Shift is closed.");

        var delta = type is CashEntryType.Withdrawal or CashEntryType.Refund ? -amount : amount;
        if (ExpectedAmount + delta < 0)
            throw new CashShiftConflictException("Insufficient cash.");

        _entries.Add(new CashEntry(Id, key, type, amount, actor, reason, paymentId));
        ExpectedAmount += delta;
        Touch();
    }

    public void Close(decimal countedAmount, string actor)
    {
        ValidateAmount(countedAmount);
        if (string.IsNullOrWhiteSpace(actor) || actor.Length > 200)
            throw new ArgumentException("Actor is required.");
        if (ClosedAt.HasValue)
        {
            if (CountedAmount != countedAmount || ClosedBy != actor)
                throw new CashShiftConflictException("Shift is already closed.");
            return;
        }
        CountedAmount = countedAmount;
        ClosedBy = actor;
        ClosedAt = DateTimeOffset.UtcNow;
        Touch();
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount < 0 || decimal.Round(amount, 2) != amount)
            throw new ArgumentException("Amount must be nonnegative with at most two decimal places.");
    }
}

public enum CashEntryType { Deposit = 1, Withdrawal = 2, Sale = 3, Refund = 4 }

public class CashEntry : AuditableEntity
{
    public Guid ShiftId { get; private set; }
    public Guid RequestKey { get; private set; }
    public CashEntryType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Actor { get; private set; } = "";
    public string Reason { get; private set; } = "";
    public Guid? PaymentId { get; private set; }

    private CashEntry() { }

    internal CashEntry(Guid shiftId, Guid requestKey, CashEntryType type,
        decimal amount, string actor, string reason, Guid? paymentId)
    {
        ShiftId = shiftId;
        RequestKey = requestKey;
        Type = type;
        Amount = amount;
        Actor = actor;
        Reason = reason;
        PaymentId = paymentId;
    }
}

public sealed class CashShiftConflictException(string message) : Exception(message);
