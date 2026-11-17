using QSR.Domain.Entities.Branches.Payments;

namespace QSR.Application.Interfaces.Repositories.Payments;

public interface IPaymentRefundRepository
{
    Task<PaymentRefund?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PaymentRefund>> GetByPaymentIdAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PaymentRefund>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PaymentRefund refund,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}