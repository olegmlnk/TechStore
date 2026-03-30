using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WishlistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsAvailable", "IsDeleted", "Price", "StockQuantity", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000001"), 0, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(2964), "Apple MacBook Pro 16-inch with M4 Max chip, 36GB RAM, 1TB SSD. The most powerful MacBook ever with stunning Liquid Retina XDR display.", "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600", true, false, 3499.00m, 25, "MacBook Pro 16\" M4 Max", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(2967) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000002"), 1, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4548), "Samsung Galaxy S25 Ultra with 6.9\" Dynamic AMOLED, Snapdragon 8 Elite, 200MP camera, 5000mAh battery, and S Pen.", "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=600", true, false, 1299.99m, 50, "Samsung Galaxy S25 Ultra", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4549) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000003"), 5, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4555), "Premium wireless noise-cancelling headphones with 40-hour battery life, Hi-Res Audio, and industry-leading ANC technology.", "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600", true, false, 399.99m, 100, "Sony WH-1000XM6", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4555) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000004"), 4, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4565), "Gaming laptop with Intel Core i9-14900HX, RTX 4090, 32GB DDR5, 2TB SSD, 18\" QHD+ 240Hz display.", "https://images.unsplash.com/photo-1593642702821-c8da6771f0c6?w=600", true, false, 2999.99m, 15, "ASUS ROG Strix G18", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4565) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000005"), 6, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4569), "The most rugged Apple Watch with 49mm titanium case, dual-frequency GPS, cellular, up to 72 hours of battery life.", "https://images.unsplash.com/photo-1546868871-af0de0ae72be?w=600", true, false, 799.99m, 40, "Apple Watch Ultra 3", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4569) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000006"), 10, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4572), "Advanced wireless mouse with MagSpeed scroll, 8K DPI sensor, ergonomic design, USB-C, works on any surface including glass.", "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=600", true, false, 109.99m, 200, "Logitech MX Master 4", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4572) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000007"), 1, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4575), "Apple iPhone 16 Pro Max with A18 Pro chip, 48MP camera system, titanium design, and all-day battery life.", "https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=600", true, false, 1199.00m, 75, "iPhone 16 Pro Max", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4575) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000008"), 10, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4578), "49-inch DQHD curved gaming monitor, 240Hz, 1ms response, Mini-LED, 1000R curvature, HDR 2000.", "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=600", true, false, 1799.99m, 10, "Samsung 49\" Odyssey G9", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4578) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000009"), 11, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4583), "Professional drone with Hasselblad camera, 8K video, omnidirectional obstacle sensing, 46-min flight time.", "https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=600", true, false, 2199.00m, 20, "DJI Mavic 4 Pro", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4584) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000010"), 12, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4586), "Full-frame mirrorless camera with 61MP sensor, AI-based autofocus, 8K video capability, 5-axis IBIS.", "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=600", true, false, 3899.99m, 12, "Sony Alpha A7R V", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4587) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000011"), 7, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4607), "Smart display with 10\" HD screen, built-in Nest Cam, Google Assistant, and smart home controls.", "https://images.unsplash.com/photo-1558089687-f282d8132c8c?w=600", true, false, 229.99m, 60, "Google Nest Hub Max", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4607) },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000012"), 9, new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4611), "Portable SSD with up to 2,000 MB/s read speed, USB 3.2, IP65 water/dust resistance, drop-proof.", "https://images.unsplash.com/photo-1597872200969-2b65d56bd16b?w=600", true, false, 349.99m, 80, "Samsung T9 Portable SSD 4TB", new DateTime(2026, 3, 29, 23, 15, 31, 790, DateTimeKind.Utc).AddTicks(4611) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_WishlistId",
                table: "WishlistItems",
                column: "WishlistId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "WishlistItems");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
