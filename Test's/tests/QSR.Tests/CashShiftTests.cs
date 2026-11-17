using QSR.Domain.Entities.Branches.Cash;

namespace QSR.Tests;

public class CashShiftTests
{
    [Fact]
    public void DuplicateEntryDoesNotChangeBalance()
    {
        var shift = CashShift.Open(Guid.NewGuid(), "POS-1", "TJS", 100, "cashier");
        var key = Guid.NewGuid();
        shift.Record(key, CashEntryType.Deposit, 20, "cashier", "Float");
        shift.Record(key, CashEntryType.Deposit, 20, "cashier", "Float");
        Assert.Equal(120, shift.ExpectedAmount);
        Assert.Single(shift.Entries);
        Assert.Throws<CashShiftConflictException>(() =>
            shift.Record(key, CashEntryType.Deposit, 30, "cashier", "Float"));
    }

    [Fact]
    public void WithdrawalCannotOverdrawCash()
    {
        var shift = CashShift.Open(Guid.NewGuid(), "POS-1", "TJS", 10, "cashier");
        Assert.Throws<CashShiftConflictException>(() =>
            shift.Record(Guid.NewGuid(), CashEntryType.Withdrawal, 11, "cashier", "Collection"));
        Assert.Equal(10, shift.ExpectedAmount);
        Assert.Empty(shift.Entries);
    }

    [Fact]
    public void CloseStoresDifferenceAndRejectsNewEntries()
    {
        var shift = CashShift.Open(Guid.NewGuid(), "POS-1", "TJS", 10, "cashier");
        shift.Close(9, "cashier");
        Assert.Equal(-1m, shift.Difference);
        shift.Close(9, "cashier");
        Assert.Throws<CashShiftConflictException>(() =>
            shift.Record(Guid.NewGuid(), CashEntryType.Deposit, 1, "cashier", "Float"));
        Assert.Throws<CashShiftConflictException>(() => shift.Close(10, "cashier"));
    }

    [Fact]
    public void SaleRequiresPaymentReference()
    {
        var shift = CashShift.Open(Guid.NewGuid(), "POS-1", "TJS", 0, "cashier");
        Assert.Throws<ArgumentException>(() =>
            shift.Record(Guid.NewGuid(), CashEntryType.Sale, 5, "cashier", "Sale"));
    }
}
