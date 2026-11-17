using QSR.Application.Interfaces.Services.Orders;

namespace QSR.Infrastructure.Services.Orders;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    public string Generate()
    {
        var date = DateTimeOffset.UtcNow
            .ToString("yyyyMMdd");

        var uniquePart = Guid.NewGuid()
            .ToString("N")
            .ToUpperInvariant();

        return $"ORD-{date}-{uniquePart}";
    }
}