using System.Security.Cryptography;
using System.Text.Json;
using QSR.Application.Interfaces.Repositories.Menu;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Interfaces.Services.Orders;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Domain.Entities.Branches.Orders;
using QSR.Domain.ValueObjects;
using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.CreateOrder;

public class CreateOrderHandler(
    IOrderRepository orderRepository,
    IBranchProductRepository branchProductRepository,
    IOrderNumberGenerator orderNumberGenerator)
{
    public async Task<CreateOrderResult> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default,
        string? creationKey = null)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.BranchId == Guid.Empty)
            throw new ArgumentException(
                "Branch ID is required.");

        if (command.Lines is null || command.Lines.Count is 0 or > 100 ||
            command.Lines.Any(x => x is null || x.Modifiers is null ||
                x.Modifiers.Count > 100 || x.Modifiers.Any(m => m is null)))
            throw new ArgumentException("Invalid order lines or modifiers.");

        if (command.Currency is not { Length: 3 } ||
            !command.Currency.All(char.IsAsciiLetter) || !Enum.IsDefined(command.Type))
            throw new ArgumentException(
                "Use a three-letter currency code and a valid order type.");

        var fingerprint = creationKey is null ? null : Convert.ToHexString(
            SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(command)));

        if (creationKey is not null &&
            await orderRepository.GetByCreationKeyAsync(
                command.BranchId, creationKey, cancellationToken) is { } existing)
        {
            if (existing.CreationFingerprint != fingerprint)
                throw new OrderCreationConflictException();

            return new CreateOrderResult(existing.Id, existing.OrderNumber,
                OrderStatus.Draft, existing.TotalAmount.Amount, existing.TotalAmount.Currency);
        }

        var productIds = command.Lines
            .Select(x => x.BranchProductId)
            .Distinct()
            .ToArray();

        var products =
            await branchProductRepository.GetByIdsAsync(
                productIds,
                cancellationToken);

        var productDictionary = products.ToDictionary(
            x => x.Id);

        var orderNumber =
            orderNumberGenerator.Generate();

        var order = Order.Create(
            command.BranchId,
            orderNumber,
            command.Type,
            command.Currency,
            command.CustomerId,
            creationKey,
            fingerprint);

        foreach (var lineCommand in command.Lines)
        {
            if (!productDictionary.TryGetValue(
                    lineCommand.BranchProductId,
                    out var product))
            {
                throw new OrderValidationException(
                    $"Product {lineCommand.BranchProductId} was not found.");
            }

            if (!product.IsActive)
            {
                throw new OrderValidationException(
                    $"Product {product.Name} is not active.");
            }

            if (!product.IsAvailable)
            {
                throw new OrderValidationException(
                    $"Product {product.Name} is not available.");
            }

            var unitPrice = product.LocalPrice;

            Guid? variantId = null;
            string? variantName = null;

            if (lineCommand.BranchProductVariantId.HasValue)
            {
                var variant = product.Variants
                    .FirstOrDefault(x =>
                        x.Id ==
                        lineCommand.BranchProductVariantId.Value);

                if (variant is null)
                {
                    throw new OrderValidationException(
                        $"Variant was not found for product {product.Name}.");
                }

                if (!variant.IsActive)
                {
                    throw new OrderValidationException(
                        $"Variant {variant.Name} is not active.");
                }

                variantId = variant.Id;
                variantName = variant.Name;
                unitPrice = variant.LocalPrice;
            }

            EnsureCurrency(
                unitPrice,
                command.Currency);

            var orderLine = OrderLine.Create(
                order.Id,
                product.Id,
                product.Name,
                lineCommand.Quantity,
                unitPrice,
                variantId,
                variantName);

            AddModifiers(
                product,
                orderLine,
                lineCommand.Modifiers,
                command.Currency);

            order.AddLine(orderLine);
        }

        await orderRepository.AddAsync(
            order,
            cancellationToken);

        await orderRepository.SaveChangesAsync(
            cancellationToken);

        return new CreateOrderResult(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency);
    }

    private static void AddModifiers(
        BranchProduct product,
        OrderLine orderLine,
        IReadOnlyCollection<CreateOrderModifierCommand> modifiers,
        string currency)
    {
        var modifierIds = modifiers
            .Select(x => x.BranchModifierOptionId)
            .ToArray();

        if (modifierIds.Length !=
            modifierIds.Distinct().Count())
        {
            throw new OrderValidationException(
                "The same modifier cannot be selected more than once.");
        }

        foreach (var group in product.ModifierGroups)
        {
            if (!group.IsActive)
                continue;

            var selectedCount = group.Options.Count(
                option => modifierIds.Contains(option.Id));

            if (group.IsRequired &&
                selectedCount == 0)
            {
                throw new OrderValidationException(
                    $"Modifier group {group.Name} is required.");
            }

            if (selectedCount < group.MinSelections)
            {
                throw new OrderValidationException(
                    $"Modifier group {group.Name} requires at least {group.MinSelections} selection(s).");
            }

            if (selectedCount > group.MaxSelections)
            {
                throw new OrderValidationException(
                    $"Modifier group {group.Name} allows maximum {group.MaxSelections} selection(s).");
            }
        }

        foreach (var modifierCommand in modifiers)
        {
            BranchModifierOption? option = null;

            foreach (var group in product.ModifierGroups)
            {
                if (!group.IsActive)
                    continue;

                option = group.Options
                    .FirstOrDefault(x =>
                        x.Id ==
                        modifierCommand.BranchModifierOptionId);

                if (option is not null)
                    break;
            }

            if (option is null)
            {
                throw new OrderValidationException(
                    $"Modifier {modifierCommand.BranchModifierOptionId} does not belong to product {product.Name}.");
            }

            if (!option.IsActive)
            {
                throw new OrderValidationException(
                    $"Modifier {option.Name} is not active.");
            }

            EnsureCurrency(
                option.LocalPriceAdjustment,
                currency);

            var orderModifier =
                OrderLineModifier.Create(
                    orderLine.Id,
                    option.Id,
                    option.Name,
                    option.LocalPriceAdjustment);

            orderLine.AddModifier(orderModifier);
        }
    }

    private static void EnsureCurrency(
        Money money,
        string currency)
    {
        if (!string.Equals(
                money.Currency,
                currency,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new OrderValidationException(
                $"Currency {money.Currency} does not match order currency {currency}.");
        }
    }
}