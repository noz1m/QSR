using QSR.Application.Interfaces.Repositories.Kitchen;
using QSR.Application.Interfaces.Persistence;
using QSR.Domain.Enums;
using QSR.Application.Interfaces.Repositories.Orders;

namespace QSR.Application.Features.Orders.MarkOrderReady;

public class MarkOrderReadyHandler(
    IOrderRepository orderRepository,
    IKitchenTaskRepository kitchenTaskRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var order = await orderRepository.GetByIdAsync(
                orderId,
                ct);

            if (order is null)
                throw new InvalidOperationException(
                    "Order was not found.");

            var tasks = await kitchenTaskRepository.GetByOrderIdAsync(orderId, ct);
            if (tasks.Count == 0 || tasks.Any(x =>
                x.Status != KitchenTaskStatus.Ready &&
                x.Status != KitchenTaskStatus.HandedOff))
                throw new InvalidOperationException("Required kitchen tasks are not ready.");

            order.MarkReady();

            await orderRepository.SaveChangesAsync(
                ct);
        }, cancellationToken);
    }
}