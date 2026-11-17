using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.MarkOrderOutForDelivery;

public class MarkOrderOutForDeliveryHandler(
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

        if (order.Type != OrderType.Delivery)
            throw new InvalidOperationException(
                "Only delivery orders can be moved to out for delivery.");

        order.MarkOutForDelivery();

        await orderRepository.SaveChangesAsync(
            cancellationToken);
    }
}