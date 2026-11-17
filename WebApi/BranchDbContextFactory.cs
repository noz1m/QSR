using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.WebApi;

public class BranchDbContextFactory : IDesignTimeDbContextFactory<BranchDbContext>
{
    public BranchDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Branch")
            ?? throw new InvalidOperationException("ConnectionStrings__Branch is required.");

        return new BranchDbContext(
            new DbContextOptionsBuilder<BranchDbContext>()
                .UseNpgsql(connectionString)
                .Options);
    }
}
