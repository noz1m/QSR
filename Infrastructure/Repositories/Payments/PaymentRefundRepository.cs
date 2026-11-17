using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Payments;
using QSR.Domain.Entities.Branches.Payments;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Payments;

public class PaymentRefundRepository(
    BranchDbContext dbContext)
    : IPaymentRefundRepository
{
    public async Task<PaymentRefund?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.PaymentRefunds
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<PaymentRefund>>
        GetByPaymentIdAsync(
            Guid paymentId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.PaymentRefunds
            .Where(x => x.PaymentId == paymentId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PaymentRefund>>
        GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.PaymentRefunds
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        PaymentRefund refund,
        CancellationToken cancellationToken = default)
    {
        await dbContext.PaymentRefunds.AddAsync(
            refund,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}