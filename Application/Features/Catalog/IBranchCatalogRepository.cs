using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Menu;

namespace QSR.Application.Features.Catalog;

public interface IBranchCatalogRepository
{
    Task<BranchMenu?> GetMenuAsync(Guid id, CancellationToken cancellationToken);
    Task<BranchCategory?> GetCategoryAsync(Guid id, CancellationToken cancellationToken);
    Task<KitchenStation?> GetStationAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<BranchMenu>> GetMenusAsync(Guid branchId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<KitchenStation>> GetStationsAsync(Guid branchId, CancellationToken cancellationToken);
    Task AddMenuAsync(BranchMenu menu, CancellationToken cancellationToken);
    Task AddStationAsync(KitchenStation station, CancellationToken cancellationToken);
    Task AddCategoryAsync(BranchCategory category, CancellationToken cancellationToken);
    Task AddProductAsync(BranchProduct product, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
}
