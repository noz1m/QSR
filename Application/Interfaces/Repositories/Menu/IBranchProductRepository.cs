using QSR.Domain.Entities.Branches.Menu;

namespace QSR.Application.Interfaces.Repositories.Menu;

public interface IBranchProductRepository
{
    Task<BranchProduct?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BranchProduct>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}