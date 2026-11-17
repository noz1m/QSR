using QSR.Application.Interfaces.Repositories.Orders;

namespace QSR.Application.Features.Orders.MarkOrderRefunded;

public class MarkOrderRefundedHandler(
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

        order.MarkRefunded();

        await orderRepository.SaveChangesAsync(
            cancellationToken);
    }
}