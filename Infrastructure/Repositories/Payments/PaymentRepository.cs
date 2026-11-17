using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Payments;
using QSR.Domain.Entities.Branches.Payments;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Payments;

public class PaymentRepository(
    BranchDbContext dbContext)
    : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Payments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Payment>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Payments
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payment);

        await dbContext.Payments.AddAsync(
            payment,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}