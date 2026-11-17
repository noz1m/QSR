using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using QSR.Infrastructure.DI;
using QSR.Infrastructure.Persistence.Branches;
using QSR.WebApi.BackgroundJobs;
using QSR.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Branch")
    ?? throw new InvalidOperationException("ConnectionStrings:Branch is required.");

builder.Services.AddDbContext<BranchDbContext>(
    options => options.UseNpgsql(connectionString));

if (args is ["--migrate"])
{
    await using var migrationHost = builder.Build();
    await using var scope = migrationHost.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<BranchDbContext>()
        .Database.MigrateAsync();
    return;
}

var authority = builder.Configuration["Authentication:Authority"];
var audience = builder.Configuration["Authentication:Audience"];

if (!Uri.TryCreate(authority, UriKind.Absolute, out var issuer) ||
    issuer.Scheme != Uri.UriSchemeHttps ||
    string.IsNullOrWhiteSpace(audience) ||
    !Guid.TryParse(builder.Configuration["Branch:Id"], out var branchId) ||
    branchId == Guid.Empty)
{
    throw new InvalidOperationException(
        "Configure the HTTPS authentication authority, audience and branch ID.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience;
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorizationBuilder()
    .AddDefaultPolicy("BranchOrders", policy => ConfigureBranchPolicy(policy, "qsr.orders"))
    .AddPolicy("BranchCatalog", policy => ConfigureBranchPolicy(policy, "qsr.catalog"))
    .AddPolicy("BranchPos", policy => ConfigureBranchPolicy(policy, "qsr.pos"))
    .AddPolicy("BranchKitchen", policy => ConfigureBranchPolicy(policy, "qsr.kitchen"))
    .AddPolicy("BranchInventory", policy => ConfigureBranchPolicy(policy, "qsr.inventory"));

void ConfigureBranchPolicy(
    Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder policy,
    string requiredScope)
{
    policy.RequireAuthenticatedUser()
        .RequireClaim("sub")
        .RequireClaim("branch_id", branchId.ToString())
        .RequireAssertion(context => context.User.FindAll("scope").Any(
            claim => claim.Value.Split(' ').Contains(requiredScope)));
}

builder.Services.AddSingleton(new BranchIdentity(branchId));
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddInfrastructure();
builder.Services.Configure<QSR.Infrastructure.Integrations.IntegrationOptions>(
    builder.Configuration.GetSection("Integrations"));
builder.Services.AddScoped<
    QSR.Application.Interfaces.Integrations.IIntegrationResolver,
    QSR.Infrastructure.Integrations.IntegrationResolver>();
builder.Services.AddHealthChecks();

if (builder.Configuration.GetConnectionString("Redis") is { Length: > 0 } redis)
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redis;
        options.InstanceName = $"qsr:{branchId:D}:";
    });
    builder.Services.AddHostedService<LowStockSnapshotJob>();
}

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/live").AllowAnonymous();

app.Run();

public sealed record BranchIdentity(Guid Id);
