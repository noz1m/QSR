using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Branches.Migrations
{
    /// <inheritdoc />
    public partial class InitialBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BranchProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceProductTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    UnavailableReason = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    KitchenStationId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreparationTimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    CentralPrice_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CentralPrice_Currency = table.Column<string>(type: "text", nullable: false),
                    LocalPrice_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LocalPrice_Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CurrentStock = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    MinimumStock = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitOfMeasureCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "kitchen_stations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kitchen_stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreationKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreationFingerprint = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchModifierGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceModifierGroupTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    MinSelections = table.Column<int>(type: "integer", nullable: false),
                    MaxSelections = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchModifierGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchModifierGroup_BranchProducts_BranchProductId",
                        column: x => x.BranchProductId,
                        principalTable: "BranchProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchProductVariant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceProductVariantTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CentralPrice_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CentralPrice_Currency = table.Column<string>(type: "text", nullable: false),
                    LocalPrice_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LocalPrice_Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchProductVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchProductVariant_BranchProducts_BranchProductId",
                        column: x => x.BranchProductId,
                        principalTable: "BranchProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchRecipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceRecipeTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchRecipes_BranchProducts_BranchProductId",
                        column: x => x.BranchProductId,
                        principalTable: "BranchProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_movements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitOfMeasureCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_movements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inventory_movements_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "kitchen_tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    KitchenStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReadyAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    HandedOffAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kitchen_tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kitchen_tasks_kitchen_stations_KitchenStationId",
                        column: x => x.KitchenStationId,
                        principalTable: "kitchen_stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kitchen_tasks_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchProductVariantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VariantName = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_lines_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Amount_Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount_Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payments_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchModifierOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchModifierGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceModifierOptionTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CentralPriceAdjustment_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CentralPriceAdjustment_Currency = table.Column<string>(type: "text", nullable: false),
                    LocalPriceAdjustment_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LocalPriceAdjustment_Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchModifierOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchModifierOption_BranchModifierGroup_BranchModifierGrou~",
                        column: x => x.BranchModifierGroupId,
                        principalTable: "BranchModifierGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchRecipeIngredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceRecipeIngredientTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchIngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitOfMeasureCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BranchRecipeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchRecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchRecipeIngredients_BranchRecipes_BranchRecipeId",
                        column: x => x.BranchRecipeId,
                        principalTable: "BranchRecipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "kitchen_task_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KitchenTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kitchen_task_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kitchen_task_items_kitchen_tasks_KitchenTaskId",
                        column: x => x.KitchenTaskId,
                        principalTable: "kitchen_tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kitchen_task_items_order_lines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "order_lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_line_modifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchModifierOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    price_adjustment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_line_modifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_line_modifiers_order_lines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "order_lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_refunds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExternalTransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_refunds_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchModifierOptionIngredient",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchModifierOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceModifierOptionIngredientTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchIngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasureCode = table.Column<string>(type: "text", nullable: false),
                    ChangeType = table.Column<int>(type: "integer", nullable: false),
                    BranchModifierOptionIngredientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchModifierOptionIngredient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchModifierOptionIngredient_BranchModifierOptionIngredie~",
                        column: x => x.BranchModifierOptionIngredientId,
                        principalTable: "BranchModifierOptionIngredient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BranchModifierOptionIngredient_BranchModifierOption_BranchM~",
                        column: x => x.BranchModifierOptionId,
                        principalTable: "BranchModifierOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchModifierGroup_BranchProductId",
                table: "BranchModifierGroup",
                column: "BranchProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchModifierOption_BranchModifierGroupId",
                table: "BranchModifierOption",
                column: "BranchModifierGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchModifierOptionIngredient_BranchModifierOptionId",
                table: "BranchModifierOptionIngredient",
                column: "BranchModifierOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchModifierOptionIngredient_BranchModifierOptionIngredie~",
                table: "BranchModifierOptionIngredient",
                column: "BranchModifierOptionIngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchProductVariant_BranchProductId",
                table: "BranchProductVariant",
                column: "BranchProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchRecipeIngredients_BranchRecipeId",
                table: "BranchRecipeIngredients",
                column: "BranchRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchRecipes_BranchProductId_Version",
                table: "BranchRecipes",
                columns: new[] { "BranchProductId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_IngredientId",
                table: "inventory_movements",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_OrderId",
                table: "inventory_movements",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_stations_BranchId",
                table: "kitchen_stations",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_stations_BranchId_Code",
                table: "kitchen_stations",
                columns: new[] { "BranchId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_stations_IsActive",
                table: "kitchen_stations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_task_items_BranchProductId",
                table: "kitchen_task_items",
                column: "BranchProductId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_task_items_KitchenTaskId",
                table: "kitchen_task_items",
                column: "KitchenTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_task_items_OrderLineId",
                table: "kitchen_task_items",
                column: "OrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_tasks_BranchId",
                table: "kitchen_tasks",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_tasks_KitchenStationId",
                table: "kitchen_tasks",
                column: "KitchenStationId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_tasks_KitchenStationId_Status",
                table: "kitchen_tasks",
                columns: new[] { "KitchenStationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_tasks_OrderId",
                table: "kitchen_tasks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_kitchen_tasks_Status",
                table: "kitchen_tasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_order_line_modifiers_BranchModifierOptionId",
                table: "order_line_modifiers",
                column: "BranchModifierOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_order_line_modifiers_OrderLineId",
                table: "order_line_modifiers",
                column: "OrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_BranchProductId",
                table: "order_lines",
                column: "BranchProductId");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_OrderId",
                table: "order_lines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_BranchId",
                table: "orders",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_BranchId_CreationKey",
                table: "orders",
                columns: new[] { "BranchId", "CreationKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerId",
                table: "orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderNumber",
                table: "orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status",
                table: "orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_payment_refunds_ExternalTransactionId",
                table: "payment_refunds",
                column: "ExternalTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_refunds_OrderId",
                table: "payment_refunds",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_refunds_PaymentId",
                table: "payment_refunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_payments_ExternalTransactionId",
                table: "payments",
                column: "ExternalTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_OrderId",
                table: "payments",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchModifierOptionIngredient");

            migrationBuilder.DropTable(
                name: "BranchProductVariant");

            migrationBuilder.DropTable(
                name: "BranchRecipeIngredients");

            migrationBuilder.DropTable(
                name: "inventory_movements");

            migrationBuilder.DropTable(
                name: "kitchen_task_items");

            migrationBuilder.DropTable(
                name: "order_line_modifiers");

            migrationBuilder.DropTable(
                name: "payment_refunds");

            migrationBuilder.DropTable(
                name: "BranchModifierOption");

            migrationBuilder.DropTable(
                name: "BranchRecipes");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "kitchen_tasks");

            migrationBuilder.DropTable(
                name: "order_lines");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "BranchModifierGroup");

            migrationBuilder.DropTable(
                name: "kitchen_stations");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "BranchProducts");
        }
    }
}
