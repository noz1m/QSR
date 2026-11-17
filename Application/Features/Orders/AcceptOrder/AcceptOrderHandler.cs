using QSR.Application.Features.Inventory.CheckOrderInventoryAvailability;
using QSR.Application.Features.Inventory.DeductIngredientsForOrder;
using QSR.Application.Interfaces.Persistence;
using QSR.Application.Interfaces.Repositories.Kitchen;
using QSR.Application.Interfaces.Repositories.Menu;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Services.Kitchen;

namespace QSR.Application.Features.Orders.AcceptOrder;

public class AcceptOrderHandler(
    IOrderRepository orderRepository,
    IBranchProductRepository branchProductRepository,
    IKitchenTaskRepository kitchenTaskRepository,
    KitchenTaskFactory kitchenTaskFactory,
    DeductIngredientsForOrderHandler deductIngredientsForOrderHandler,
    CheckOrderInventoryAvailabilityHandler checkInventoryHandler,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID is required.");
        }

        await unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var order = await orderRepository.GetByIdAsync(
                    orderId,
                    ct);

                if (order is null)
                {
                    throw new InvalidOperationException(
                        "Order was not found.");
                }

                var existingTasks =
                    await kitchenTaskRepository.GetByOrderIdAsync(
                        orderId,
                        ct);

                if (existingTasks.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Kitchen tasks already exist for this order.");
                }

                var productIds = order.Lines
                    .Select(x => x.BranchProductId)
                    .Distinct()
                    .ToArray();

                var products =
                    await branchProductRepository.GetByIdsAsync(
                        productIds,
                        ct);

                if (products.Count != productIds.Length)
                {
                    throw new InvalidOperationException(
                        "One or more products were not found.");
                }

                var inventoryAvailability =
                    await checkInventoryHandler.HandleAsync(
                        order.Id,
                        ct);

                if (!inventoryAvailability.IsAvailable)
                {
                    var shortages = string.Join(
                        ", ",
                        inventoryAvailability.Shortages.Select(x =>
                            $"{x.IngredientName}: " +
                            $"required {x.RequiredQuantity} {x.UnitOfMeasureCode}, " +
                            $"available {x.AvailableQuantity} {x.UnitOfMeasureCode}"));

                    throw new InvalidOperationException(
                        $"Insufficient ingredients. {shortages}");
                }

                await deductIngredientsForOrderHandler.HandleAsync(
                    order.Id,
                    ct);

                order.Accept();

                var kitchenTasks =
                    kitchenTaskFactory.CreateTasks(
                        order,
                        products);

                await kitchenTaskRepository.AddRangeAsync(
                    kitchenTasks,
                    ct);

                await orderRepository.SaveChangesAsync(
                    ct);
            },
            cancellationToken);
    }
}