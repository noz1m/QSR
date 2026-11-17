using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.MarkOrderAwaitingPickup;

public class MarkOrderAwaitingPickupHandler(
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

        if (order.Type != OrderType.Pickup)
            throw new InvalidOperationException(
                "Only pickup orders can be moved to awaiting pickup.");

        order.MarkAwaitingPickup();

        await orderRepository.SaveChangesAsync(
            cancellationToken);
    }
}