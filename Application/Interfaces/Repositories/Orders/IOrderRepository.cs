using QSR.Domain.Entities.Branches.Orders;

namespace QSR.Application.Interfaces.Repositories.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByCreationKeyAsync(
        Guid branchId, string key, CancellationToken cancellationToken = default);

    Task<Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Order>> GetByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}