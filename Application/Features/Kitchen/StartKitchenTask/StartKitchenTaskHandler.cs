using QSR.Application.Interfaces.Persistence;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Domain.Enums;
using QSR.Application.Interfaces.Repositories.Kitchen;

namespace QSR.Application.Features.Kitchen.StartKitchenTask;

public class StartKitchenTaskHandler(
    IKitchenTaskRepository kitchenTaskRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        Guid kitchenTaskId,
        CancellationToken cancellationToken = default)
    {
        if (kitchenTaskId == Guid.Empty)
            throw new ArgumentException(
                "Kitchen task ID is required.");

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var kitchenTask =
                await kitchenTaskRepository.GetByIdAsync(
                    kitchenTaskId,
                    ct);

            if (kitchenTask is null)
                throw new InvalidOperationException(
                    "Kitchen task was not found.");

                var order = await orderRepository.GetByIdAsync(kitchenTask.OrderId, ct)
                    ?? throw new InvalidOperationException("Order was not found.");

                if (order.Status == OrderStatus.Accepted)
                    order.StartPreparing();
                else if (order.Status != OrderStatus.Preparing)
                    throw new InvalidOperationException("Order cannot enter preparation.");

            kitchenTask.StartPreparing(
                DateTimeOffset.UtcNow);

            await kitchenTaskRepository.SaveChangesAsync(
                ct);
        }, cancellationToken);
    }
}