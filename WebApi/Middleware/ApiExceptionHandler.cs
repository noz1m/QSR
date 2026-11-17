using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace QSR.WebApi.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            QSR.Application.Features.Orders.CreateOrder.OrderValidationException
                => StatusCodes.Status422UnprocessableEntity,
            QSR.Application.Interfaces.Integrations.IntegrationUnavailableException
                => StatusCodes.Status503ServiceUnavailable,
            QSR.Application.Features.Recipes.CreateRecipe.RecipeVersionConflictException
                => StatusCodes.Status409Conflict,
            QSR.Domain.Entities.Branches.Cash.CashShiftConflictException
                => StatusCodes.Status409Conflict,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            QSR.Application.Features.Orders.CreateOrder.OrderCreationConflictException
                => StatusCodes.Status409Conflict,
            PostgresException { SqlState: PostgresErrorCodes.SerializationFailure }
                => StatusCodes.Status409Conflict,
            DbUpdateException { InnerException: PostgresException
                { SqlState: PostgresErrorCodes.SerializationFailure } }
                => StatusCodes.Status409Conflict,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            DbUpdateException { InnerException: PostgresException
                { SqlState: PostgresErrorCodes.UniqueViolation } }
                => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled request failure. Trace: {TraceId}", context.TraceIdentifier);

        await Results.Problem(statusCode: statusCode).ExecuteAsync(context);
        return true;
    }
}
