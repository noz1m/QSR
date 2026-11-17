using Microsoft.EntityFrameworkCore;
using QSR.Application.Features.Cash;
using QSR.Domain.Entities.Branches.Cash;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Cash;

public class CashShiftRepository(BranchDbContext dbContext) : ICashShiftRepository
{
    public Task<CashShift?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<CashShift>().Include(x => x.Entries)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<CashShift?> GetOpenAsync(Guid branchId, string registerCode,
        CancellationToken cancellationToken)
    {
        return dbContext.Set<CashShift>().Include(x => x.Entries)
            .SingleOrDefaultAsync(x => x.BranchId == branchId &&
                x.RegisterCode == registerCode && x.ClosedAt == null, cancellationToken);
    }

    public async Task AddAsync(CashShift shift, CancellationToken cancellationToken)
    {
        await dbContext.Set<CashShift>().AddAsync(shift, cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
