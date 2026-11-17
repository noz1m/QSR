using Microsoft.Extensions.DependencyInjection;

using QSR.Application.Features.Inventory.AddIngredientStock;
using QSR.Application.Features.Inventory.DeductIngredientStock;
using QSR.Application.Features.Inventory.GetInventoryMovements;

using QSR.Application.Features.Kitchen.AcceptKitchenTask;
using QSR.Application.Features.Kitchen.CancelKitchenTask;
using QSR.Application.Features.Kitchen.HandOffKitchenTask;
using QSR.Application.Features.Kitchen.MarkKitchenTaskReady;
using QSR.Application.Features.Kitchen.StartKitchenTask;

using QSR.Application.Services.Kitchen;

using QSR.Infrastructure.Services.Orders;

using QSR.Application.Features.Orders.AcceptOrder;
using QSR.Application.Features.Orders.CancelOrder;
using QSR.Application.Features.Orders.CompleteOrder;
using QSR.Application.Features.Orders.CreateOrder;
using QSR.Application.Features.Orders.GetOrderById;
using QSR.Application.Features.Orders.GetOrdersByBranch;
using QSR.Application.Features.Orders.MarkOrderAwaitingPickup;
using QSR.Application.Features.Orders.MarkOrderOutForDelivery;
using QSR.Application.Features.Orders.MarkOrderPaid;
using QSR.Application.Features.Orders.MarkOrderPartiallyPaid;
using QSR.Application.Features.Orders.MarkOrderPartiallyRefunded;
using QSR.Application.Features.Orders.MarkOrderReady;
using QSR.Application.Features.Orders.MarkOrderRefunded;
using QSR.Application.Features.Orders.MarkRefundPending;
using QSR.Application.Features.Orders.SubmitOrderForPayment;

using QSR.Application.Features.Payments.AddPayment;
using QSR.Application.Features.Payments.GetPaymentsByOrder;
using QSR.Application.Features.Payments.RefundPayment;

using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Application.Interfaces.Repositories.Kitchen;
using QSR.Application.Interfaces.Repositories.Menu;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Interfaces.Repositories.Payments;
using QSR.Application.Interfaces.Services.Orders;
using QSR.Application.Interfaces.Repositories.Recipes;

using QSR.Infrastructure.Repositories.Inventory;
using QSR.Infrastructure.Repositories.Kitchen;
using QSR.Infrastructure.Repositories.Menu;
using QSR.Infrastructure.Repositories.Orders;
using QSR.Infrastructure.Repositories.Payments;
using QSR.Infrastructure.Repositories.Recipes;

using QSR.Application.Features.Inventory.DeductIngredientsForOrder;
using QSR.Application.Services.Recipes;

using QSR.Application.Interfaces.Persistence;
using QSR.Infrastructure.Persistence.Branches;

using QSR.Application.Features.Inventory.CheckProductAvailability;

using QSR.Application.Features.Inventory.CheckOrderInventoryAvailability;

using QSR.Application.Features.Inventory.GetLowStockIngredients;

using QSR.Application.Features.Inventory.AdjustIngredientStock;
using QSR.Application.Features.Inventory.WasteIngredient;
using QSR.Application.Features.Inventory.RestoreIngredientsForOrder;

using QSR.Application.Features.Payments.GetPaymentRefunds;
using QSR.Application.Features.Inventory.CreateIngredient;

using QSR.Application.Features.Inventory.ActivateIngredient;
using QSR.Application.Features.Inventory.DeactivateIngredient;
using QSR.Application.Features.Inventory.UpdateIngredient;

using QSR.Application.Features.Recipes.GetRecipeByProduct;


using QSR.Application.Features.Inventory.GetIngredients;

namespace QSR.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<QSR.Application.Features.Recipes.CreateRecipe.CreateRecipeHandler>();
        services.AddScoped<QSR.Application.Features.Cash.CashShiftHandler>();
        services.AddScoped<QSR.Application.Features.Cash.ICashShiftRepository,
            QSR.Infrastructure.Repositories.Cash.CashShiftRepository>();
        services.AddScoped<QSR.Application.Features.Catalog.BranchCatalogHandler>();
        services.AddScoped<QSR.Application.Features.Catalog.IBranchCatalogRepository,
            QSR.Infrastructure.Repositories.Catalog.BranchCatalogRepository>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        services.AddScoped<GetOrdersByBranchHandler>();

        services.AddScoped<SubmitOrderForPaymentHandler>();
        services.AddScoped<MarkOrderPartiallyPaidHandler>();
        services.AddScoped<MarkOrderPaidHandler>();
        services.AddScoped<AcceptOrderHandler>();

        services.AddScoped<MarkOrderReadyHandler>();
        services.AddScoped<MarkOrderAwaitingPickupHandler>();
        services.AddScoped<MarkOrderOutForDeliveryHandler>();
        services.AddScoped<CompleteOrderHandler>();
        services.AddScoped<CancelOrderHandler>();

        services.AddScoped<MarkRefundPendingHandler>();
        services.AddScoped<MarkOrderPartiallyRefundedHandler>();
        services.AddScoped<MarkOrderRefundedHandler>();

        services.AddScoped<AcceptKitchenTaskHandler>();
        services.AddScoped<StartKitchenTaskHandler>();
        services.AddScoped<MarkKitchenTaskReadyHandler>();
        services.AddScoped<HandOffKitchenTaskHandler>();
        services.AddScoped<CancelKitchenTaskHandler>();

        services.AddScoped<AddPaymentHandler>();
        services.AddScoped<GetPaymentsByOrderHandler>();
        services.AddScoped<RefundPaymentHandler>();

        services.AddScoped<AddIngredientStockHandler>();
        services.AddScoped<DeductIngredientStockHandler>();

        services.AddScoped<GetInventoryMovementsByIngredientHandler>();
        services.AddScoped<GetInventoryMovementsByOrderHandler>();

        services.AddScoped<KitchenTaskFactory>();

        services.AddScoped<DeductIngredientsForOrderHandler>();

        services.AddScoped<RecipeConsumptionCalculator>();
        services.AddScoped<CheckProductAvailabilityHandler>();

        services.AddScoped<CheckOrderInventoryAvailabilityHandler>();
        services.AddScoped<GetLowStockIngredientsHandler>();

        services.AddScoped<AdjustIngredientStockHandler>();
        services.AddScoped<WasteIngredientHandler>();
        services.AddScoped<RestoreIngredientsForOrderHandler>();


        services.AddScoped<GetPaymentRefundsByPaymentHandler>();
        services.AddScoped<GetPaymentRefundsByOrderHandler>();
        services.AddScoped<CreateIngredientHandler>();

        services.AddScoped<UpdateIngredientHandler>();
        services.AddScoped<ActivateIngredientHandler>();
        services.AddScoped<DeactivateIngredientHandler>();

        services.AddScoped<GetIngredientByIdHandler>();
        services.AddScoped<GetIngredientsHandler>();
    
        services.AddScoped<GetRecipeByProductHandler>();

        services.AddScoped<
            IPaymentRefundRepository,
            PaymentRefundRepository>();


        services.AddScoped<
            IUnitOfWork,
            UnitOfWork>();

        services.AddScoped<
            IBranchRecipeRepository,
            BranchRecipeRepository>();

        services.AddScoped<
            IOrderRepository,
            OrderRepository>();

        services.AddScoped<
            IBranchProductRepository,
            BranchProductRepository>();

        services.AddScoped<
            IKitchenTaskRepository,
            KitchenTaskRepository>();

        services.AddScoped<
            IPaymentRepository,
            PaymentRepository>();

        services.AddScoped<
            IIngredientRepository,
            IngredientRepository>();

        services.AddScoped<
            IInventoryMovementRepository,
            InventoryMovementRepository>();

        services.AddScoped<
            IOrderNumberGenerator,
            OrderNumberGenerator>();

        return services;
    }
}