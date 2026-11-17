using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Domain.Entities.Branches.Orders;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Orders;

public class OrderRepository(
    BranchDbContext dbContext)
    : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .Include(x => x.Lines)
                .ThenInclude(x => x.Modifiers)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<Order?> GetByCreationKeyAsync(
        Guid branchId,
        string key,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(
            x => x.BranchId == branchId && x.CreationKey == key,
            cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .Include(x => x.Lines)
                .ThenInclude(x => x.Modifiers)
            .FirstOrDefaultAsync(
                x => x.OrderNumber == orderNumber,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .Where(x => x.BranchId == branchId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        await dbContext.Orders.AddAsync(
            order,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}