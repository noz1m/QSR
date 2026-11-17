using QSR.Application.Interfaces.Repositories.Orders;

namespace QSR.Application.Features.Orders.MarkOrderPartiallyRefunded;

public class MarkOrderPartiallyRefundedHandler(
    IOrderRepository orderRepository)
{
    public async Task HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        var order = await orderRepository.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                "Order was not found.");

        order.MarkPartiallyRefunded();

        await orderRepository.SaveChangesAsync(
            cancellationToken);
    }
}