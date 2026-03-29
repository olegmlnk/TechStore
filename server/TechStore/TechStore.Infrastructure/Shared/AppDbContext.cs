using Microsoft.EntityFrameworkCore;
using TechStore.Core.Enums;
using TechStore.Core.Models;

namespace TechStore.Infrastructure.Shared;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasMany(u => u.Addresses).WithOne(a => a.User).HasForeignKey(a => a.UserId);
            e.HasMany(u => u.Orders).WithOne(o => o.User).HasForeignKey(o => o.UserId);
            e.HasOne(u => u.Cart).WithOne(c => c.User).HasForeignKey<Cart>(c => c.UserId);
            e.HasOne(u => u.Wishlist).WithOne(w => w.User).HasForeignKey<Wishlist>(w => w.UserId);
        });

        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId);
        });

        // Cart
        modelBuilder.Entity<Cart>(e =>
        {
            e.HasMany(c => c.Items).WithOne(i => i.Cart).HasForeignKey(i => i.CartId);
        });

        // Wishlist
        modelBuilder.Entity<Wishlist>(e =>
        {
            e.HasMany(w => w.Items).WithOne(i => i.Wishlist).HasForeignKey(i => i.WishlistId);
        });

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Cart>().HasQueryFilter(e => !e.IsDeleted);

        // Decimal precision
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Cart>().Property(c => c.TotalPrice).HasPrecision(18, 2);

        // Seed products
        SeedProducts(modelBuilder);
    }

    private static void SeedProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"),
                Title = "MacBook Pro 16\" M4 Max",
                Description = "Apple MacBook Pro 16-inch with M4 Max chip, 36GB RAM, 1TB SSD. The most powerful MacBook ever with stunning Liquid Retina XDR display.",
                Price = 3499.00m,
                IsAvailable = true,
                StockQuantity = 25,
                Category = Category.Computers,
                ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"),
                Title = "Samsung Galaxy S25 Ultra",
                Description = "Samsung Galaxy S25 Ultra with 6.9\" Dynamic AMOLED, Snapdragon 8 Elite, 200MP camera, 5000mAh battery, and S Pen.",
                Price = 1299.99m,
                IsAvailable = true,
                StockQuantity = 50,
                Category = Category.Phones,
                ImageUrl = "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"),
                Title = "Sony WH-1000XM6",
                Description = "Premium wireless noise-cancelling headphones with 40-hour battery life, Hi-Res Audio, and industry-leading ANC technology.",
                Price = 399.99m,
                IsAvailable = true,
                StockQuantity = 100,
                Category = Category.Audio,
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000004"),
                Title = "ASUS ROG Strix G18",
                Description = "Gaming laptop with Intel Core i9-14900HX, RTX 4090, 32GB DDR5, 2TB SSD, 18\" QHD+ 240Hz display.",
                Price = 2999.99m,
                IsAvailable = true,
                StockQuantity = 15,
                Category = Category.Gaming,
                ImageUrl = "https://images.unsplash.com/photo-1593642702821-c8da6771f0c6?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000005"),
                Title = "Apple Watch Ultra 3",
                Description = "The most rugged Apple Watch with 49mm titanium case, dual-frequency GPS, cellular, up to 72 hours of battery life.",
                Price = 799.99m,
                IsAvailable = true,
                StockQuantity = 40,
                Category = Category.Wearables,
                ImageUrl = "https://images.unsplash.com/photo-1546868871-af0de0ae72be?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000006"),
                Title = "Logitech MX Master 4",
                Description = "Advanced wireless mouse with MagSpeed scroll, 8K DPI sensor, ergonomic design, USB-C, works on any surface including glass.",
                Price = 109.99m,
                IsAvailable = true,
                StockQuantity = 200,
                Category = Category.Peripherals,
                ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000007"),
                Title = "iPhone 16 Pro Max",
                Description = "Apple iPhone 16 Pro Max with A18 Pro chip, 48MP camera system, titanium design, and all-day battery life.",
                Price = 1199.00m,
                IsAvailable = true,
                StockQuantity = 75,
                Category = Category.Phones,
                ImageUrl = "https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000008"),
                Title = "Samsung 49\" Odyssey G9",
                Description = "49-inch DQHD curved gaming monitor, 240Hz, 1ms response, Mini-LED, 1000R curvature, HDR 2000.",
                Price = 1799.99m,
                IsAvailable = true,
                StockQuantity = 10,
                Category = Category.Peripherals,
                ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000009"),
                Title = "DJI Mavic 4 Pro",
                Description = "Professional drone with Hasselblad camera, 8K video, omnidirectional obstacle sensing, 46-min flight time.",
                Price = 2199.00m,
                IsAvailable = true,
                StockQuantity = 20,
                Category = Category.Drones,
                ImageUrl = "https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000010"),
                Title = "Sony Alpha A7R V",
                Description = "Full-frame mirrorless camera with 61MP sensor, AI-based autofocus, 8K video capability, 5-axis IBIS.",
                Price = 3899.99m,
                IsAvailable = true,
                StockQuantity = 12,
                Category = Category.Cameras,
                ImageUrl = "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000011"),
                Title = "Google Nest Hub Max",
                Description = "Smart display with 10\" HD screen, built-in Nest Cam, Google Assistant, and smart home controls.",
                Price = 229.99m,
                IsAvailable = true,
                StockQuantity = 60,
                Category = Category.SmartHome,
                ImageUrl = "https://images.unsplash.com/photo-1558089687-f282d8132c8c?w=600"
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000012"),
                Title = "Samsung T9 Portable SSD 4TB",
                Description = "Portable SSD with up to 2,000 MB/s read speed, USB 3.2, IP65 water/dust resistance, drop-proof.",
                Price = 349.99m,
                IsAvailable = true,
                StockQuantity = 80,
                Category = Category.Storage,
                ImageUrl = "https://images.unsplash.com/photo-1597872200969-2b65d56bd16b?w=600"
            }
        );
    }
}