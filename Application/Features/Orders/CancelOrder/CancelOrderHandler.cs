using QSR.Application.Features.Inventory.RestoreIngredientsForOrder;
using QSR.Application.Interfaces.Persistence;
using QSR.Application.Interfaces.Repositories.Kitchen;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.CancelOrder;

public class CancelOrderHandler(
    IOrderRepository orderRepository,
    IKitchenTaskRepository kitchenTaskRepository,
    RestoreIngredientsForOrderHandler restoreIngredientsForOrderHandler,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        Guid orderId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID is required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "Cancellation reason is required.");
        }

        await unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var order =
                    await orderRepository.GetByIdAsync(
                        orderId,
                        ct);

                if (order is null)
                {
                    throw new InvalidOperationException(
                        "Order was not found.");
                }

                var kitchenTasks =
                    await kitchenTaskRepository.GetByOrderIdAsync(
                        orderId,
                        ct);

                var preparationStarted =
                    kitchenTasks.Any(x =>
                        x.Status == KitchenTaskStatus.Preparing ||
                        x.Status == KitchenTaskStatus.Ready ||
                        x.Status == KitchenTaskStatus.HandedOff);

                foreach (var kitchenTask in kitchenTasks)
                {
                    if (kitchenTask.Status == KitchenTaskStatus.Cancelled ||
                        kitchenTask.Status == KitchenTaskStatus.HandedOff)
                    {
                        continue;
                    }

                    if (kitchenTask.Status == KitchenTaskStatus.Ready)
                    {
                        continue;
                    }

                    kitchenTask.Cancel(
                        reason,
                        DateTimeOffset.UtcNow);
                }

                if (!preparationStarted)
                {
                    try
                    {
                        await restoreIngredientsForOrderHandler.HandleAsync(
                            order.Id,
                            reason,
                            ct);
                    }
                    catch (InvalidOperationException exception)
                        when (
                            exception.Message ==
                            "No inventory consumption was found for this order.")
                    {
                        // Order may have been cancelled
                        // before inventory was deducted.
                    }
                }

                order.Cancel();

                await orderRepository.SaveChangesAsync(
                    ct);
            },
            cancellationToken);
    }
}