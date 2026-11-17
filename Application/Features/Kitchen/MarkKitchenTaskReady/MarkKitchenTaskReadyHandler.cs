using QSR.Application.Interfaces.Persistence;
using QSR.Application.Interfaces.Repositories.Kitchen;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Kitchen.MarkKitchenTaskReady;

public class MarkKitchenTaskReadyHandler(
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

            kitchenTask.MarkReady(
                DateTimeOffset.UtcNow);

            var orderTasks =
                await kitchenTaskRepository.GetByOrderIdAsync(
                    kitchenTask.OrderId,
                    ct);

            var allTasksReady = orderTasks.All(x =>
                x.Id == kitchenTask.Id ||
                x.Status == KitchenTaskStatus.Ready ||
                x.Status == KitchenTaskStatus.HandedOff);

            if (allTasksReady)
            {
                var order = await orderRepository.GetByIdAsync(
                    kitchenTask.OrderId,
                    ct);

                if (order is null)
                    throw new InvalidOperationException(
                        "Order was not found.");

                order.MarkAwaitingAssembly();
            }

            await kitchenTaskRepository.SaveChangesAsync(
                ct);
        }, cancellationToken);
    }
}