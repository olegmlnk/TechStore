using Microsoft.EntityFrameworkCore;
using TechStore.Application.DTOs;
using TechStore.Core.Models;
using TechStore.Infrastructure.Shared;

namespace TechStore.Application.Services;

public class CartService
{
    private readonly AppDbContext _db;

    public CartService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CartResponse> GetCartAsync(Guid userId)
    {
        var cart = await GetOrCreateCartEntityAsync(userId);
        if (_db.Entry(cart).State == EntityState.Added)
            await _db.SaveChangesAsync();
        return MapToResponse(cart);
    }

    public async Task<CartResponse> AddToCartAsync(Guid userId, AddToCartRequest req)
    {
        var cart = await GetOrCreateCartEntityAsync(userId);

        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == req.ProductId);
        if (product == null || !product.IsAvailable || product.StockQuantity < req.Quantity)
            throw new InvalidOperationException("Product is not available or out of stock.");

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == req.ProductId);
        if (existingItem != null)
        {
            if (existingItem.Quantity + req.Quantity > product.StockQuantity)
                throw new InvalidOperationException("Not enough stock available.");

            existingItem.Quantity += req.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = req.ProductId,
                Quantity = req.Quantity
            });
        }

        // SaveChanges only touches CartItem (INSERT/UPDATE) and Cart INSERT for new carts.
        // We never mutate Cart.TotalPrice through the change tracker, which avoids the
        // DbUpdateConcurrencyException caused by EF Core 10's Cart UPDATE path.
        await _db.SaveChangesAsync();

        // Recalculate and persist Cart.TotalPrice via ExecuteUpdateAsync (clean SQL,
        // no rows-affected concurrency check).
        await SyncCartTotalAsync(cart.Id);

        return MapToResponse(await LoadCartWithProductsAsync(cart.Id));
    }

    public async Task<CartResponse> UpdateQuantityAsync(Guid userId, Guid itemId, UpdateCartItemRequest req)
    {
        var cart = await GetOrCreateCartEntityAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

        if (item == null)
            throw new KeyNotFoundException("Item not found in cart.");

        if (item.Product == null)
            throw new InvalidOperationException("Product is no longer available.");

        if (req.Quantity > item.Product.StockQuantity)
            throw new InvalidOperationException("Requested quantity exceeds available stock.");

        item.Quantity = req.Quantity;
        await _db.SaveChangesAsync();
        await SyncCartTotalAsync(cart.Id);

        return MapToResponse(await LoadCartWithProductsAsync(cart.Id));
    }

    public async Task<CartResponse> RemoveItemAsync(Guid userId, Guid itemId)
    {
        var cart = await GetOrCreateCartEntityAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

        if (item == null)
            return MapToResponse(cart);

        cart.Items.Remove(item);
        await _db.SaveChangesAsync();
        await SyncCartTotalAsync(cart.Id);

        return MapToResponse(await LoadCartWithProductsAsync(cart.Id));
    }

    public async Task<CartResponse> ClearCartAsync(Guid userId)
    {
        var cart = await GetOrCreateCartEntityAsync(userId);

        if (!cart.Items.Any())
            return MapToResponse(cart);

        _db.CartItems.RemoveRange(cart.Items);
        await _db.SaveChangesAsync();

        await _db.Carts
            .IgnoreQueryFilters()
            .Where(c => c.Id == cart.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.TotalPrice, 0m));

        return MapToResponse(await LoadCartWithProductsAsync(cart.Id));
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task<Cart> GetOrCreateCartEntityAsync(Guid userId)
    {
        var cart = await _db.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId, TotalPrice = 0 };
            _db.Carts.Add(cart);
        }

        return cart;
    }

    /// <summary>
    /// Recalculates the cart total directly from the DB (joining CartItems × Products)
    /// and persists it via ExecuteUpdateAsync, bypassing EF change tracking entirely.
    /// </summary>
    private async Task SyncCartTotalAsync(Guid cartId)
    {
        var total = await _db.CartItems
            .Where(i => i.CartId == cartId)
            .Join(_db.Products,          // Products global filter (!IsDeleted) applies here
                  i => i.ProductId,
                  p => p.Id,
                  (i, p) => i.Quantity * p.Price)
            .SumAsync();

        await _db.Carts
            .IgnoreQueryFilters()        // Update the Cart row regardless of IsDeleted state
            .Where(c => c.Id == cartId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.TotalPrice, total));
    }

    /// <summary>
    /// Always loads a fresh (AsNoTracking) cart from the DB so we pick up the
    /// TotalPrice written by SyncCartTotalAsync rather than a stale tracked value.
    /// </summary>
    private async Task<Cart> LoadCartWithProductsAsync(Guid cartId)
    {
        return await _db.Carts
            .AsNoTracking()
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstAsync(c => c.Id == cartId);
    }

    private CartResponse MapToResponse(Cart cart)
    {
        return new CartResponse
        {
            Id = cart.Id,
            TotalPrice = cart.TotalPrice,
            Items = cart.Items
                .Where(i => i.Product != null)
                .Select(i => new CartItemResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductTitle = i.Product!.Title,
                    ProductImageUrl = i.Product.ImageUrl,
                    UnitPrice = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
        };
    }
}
