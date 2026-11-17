using QSR.Domain.Entities.Branches.Payments;

namespace QSR.Application.Interfaces.Repositories.Payments;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Payment>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}