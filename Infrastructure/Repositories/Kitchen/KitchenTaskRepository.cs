using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Kitchen;
using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Kitchen;

public class KitchenTaskRepository(
    BranchDbContext dbContext)
    : IKitchenTaskRepository
{
    public async Task<IReadOnlyCollection<KitchenTask>> GetStationQueueAsync(
        Guid branchId, Guid stationId, CancellationToken cancellationToken = default)
    {
        return await dbContext.KitchenTasks.AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.BranchId == branchId && x.KitchenStationId == stationId &&
                x.Status != QSR.Domain.Enums.KitchenTaskStatus.Cancelled &&
                x.Status != QSR.Domain.Enums.KitchenTaskStatus.HandedOff)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<KitchenTask?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.KitchenTasks
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<KitchenTask>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.KitchenTasks
            .Include(x => x.Items)
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<KitchenTask> tasks,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        await dbContext.KitchenTasks.AddRangeAsync(
            tasks,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}