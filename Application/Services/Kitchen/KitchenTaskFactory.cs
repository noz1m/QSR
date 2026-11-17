using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Domain.Entities.Branches.Orders;

namespace QSR.Application.Services.Kitchen;

public class KitchenTaskFactory
{
    public IReadOnlyCollection<KitchenTask> CreateTasks(
        Order order,
        IEnumerable<BranchProduct> products)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(products);

        var productDictionary = products.ToDictionary(
            x => x.Id);

        var tasks = new Dictionary<Guid, KitchenTask>();

        foreach (var orderLine in order.Lines)
        {
            if (!productDictionary.TryGetValue(
                    orderLine.BranchProductId,
                    out var product))
            {
                throw new InvalidOperationException(
                    $"Product {orderLine.BranchProductId} was not found.");
            }

            if (product.KitchenStationId is null)
            {
                throw new InvalidOperationException(
                    $"Product {product.Name} does not have a kitchen station.");
            }

            var kitchenStationId = product.KitchenStationId.Value;

            if (!tasks.TryGetValue(
                    kitchenStationId,
                    out var kitchenTask))
            {
                kitchenTask = KitchenTask.Create(
                    order.BranchId,
                    order.Id,
                    kitchenStationId);

                tasks.Add(
                    kitchenStationId,
                    kitchenTask);
            }

            var item = KitchenTaskItem.Create(
                kitchenTask.Id,
                orderLine.Id,
                orderLine.BranchProductId,
                orderLine.ProductName,
                orderLine.Quantity);

            kitchenTask.AddItem(item);
        }

        return tasks.Values.ToArray();
    }
}