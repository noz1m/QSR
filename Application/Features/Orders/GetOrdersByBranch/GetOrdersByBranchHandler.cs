using QSR.Application.Interfaces.Repositories.Orders;

namespace QSR.Application.Features.Orders.GetOrdersByBranch;

public class GetOrdersByBranchHandler(
    IOrderRepository orderRepository)
{
    public async Task<IReadOnlyCollection<GetOrdersByBranchResult>> HandleAsync(
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException(
                "Branch ID is required.");

        var orders = await orderRepository.GetByBranchIdAsync(
            branchId,
            cancellationToken);

        return orders
            .Select(order =>
                new GetOrdersByBranchResult(
                    order.Id,
                    order.OrderNumber,
                    order.Type,
                    order.Status,
                    order.TotalAmount.Amount,
                    order.TotalAmount.Currency,
                    order.CreatedAt))
            .ToArray();
    }
}