using QSR.Domain.Entities.Branches.Kitchen;

namespace QSR.Application.Interfaces.Repositories.Kitchen;

public interface IKitchenTaskRepository
{
    Task<IReadOnlyCollection<KitchenTask>> GetStationQueueAsync(
        Guid branchId, Guid stationId, CancellationToken cancellationToken = default);

    Task<KitchenTask?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<KitchenTask>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<KitchenTask> tasks,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}