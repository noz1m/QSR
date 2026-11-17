using QSR.Application.Features.Payments.AddPayment;
using QSR.Application.Interfaces.Persistence;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Domain.Entities.Branches.Cash;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Cash;

public class CashShiftHandler(
    ICashShiftRepository repository,
    IOrderRepository orders,
    AddPaymentHandler payments,
    IUnitOfWork unitOfWork)
{
    public async Task<CashShift> OpenAsync(Guid branchId, string registerCode,
        string currency, decimal amount, string actor, CancellationToken cancellationToken)
    {
        var shift = CashShift.Open(branchId, registerCode, currency, amount, actor);
        if (await repository.GetOpenAsync(branchId, shift.RegisterCode, cancellationToken) is not null)
            throw new CashShiftConflictException("Register already has an open shift.");
        await repository.AddAsync(shift, cancellationToken);
        await repository.SaveAsync(cancellationToken);
        return shift;
    }

    public async Task<CashShift> GetAsync(Guid branchId, Guid shiftId,
        CancellationToken cancellationToken)
    {
        var shift = await repository.GetAsync(shiftId, cancellationToken);
        if (shift is null || shift.BranchId != branchId)
            throw new KeyNotFoundException("Shift was not found.");
        return shift;
    }

    public async Task RecordAsync(Guid branchId, Guid shiftId, Guid key,
        CashEntryType type, decimal amount, string actor, string reason,
        CancellationToken cancellationToken)
    {
        if (type is not (CashEntryType.Deposit or CashEntryType.Withdrawal))
            throw new ArgumentException("Only cash deposits and withdrawals are supported here.");
        var shift = await GetAsync(branchId, shiftId, cancellationToken);
        shift.Record(key, type, amount, actor, reason);
        await repository.SaveAsync(cancellationToken);
    }

    public async Task CloseAsync(Guid branchId, Guid shiftId, decimal amount,
        string actor, CancellationToken cancellationToken)
    {
        var shift = await GetAsync(branchId, shiftId, cancellationToken);
        shift.Close(amount, actor);
        await repository.SaveAsync(cancellationToken);
    }

    public async Task<AddPaymentResult> PayAsync(Guid branchId, Guid shiftId,
        Guid orderId, Guid key, decimal amount, string actor, CancellationToken cancellationToken)
    {
        if (key == Guid.Empty)
            throw new ArgumentException("Payment key is required.");

        AddPaymentResult? result = null;
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var shift = await GetAsync(branchId, shiftId, ct);
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null || order.BranchId != branchId)
                throw new KeyNotFoundException("Order was not found.");
            if (order.TotalAmount.Currency != shift.Currency)
                throw new CashShiftConflictException("Shift and order currencies differ.");
            result = await payments.HandleAsync(new AddPaymentCommand(orderId,
                PaymentMethod.Cash, amount, shift.Currency, $"cash:{key:N}"), ct);
            shift.Record(key, CashEntryType.Sale, amount, actor, "Cash payment", result.PaymentId);
            await repository.SaveAsync(ct);
        }, cancellationToken);

        return result ?? throw new CashShiftConflictException("Payment did not complete.");
    }
}
