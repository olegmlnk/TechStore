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
        var cartId = await EnsureCartIdAsync(userId);
        return await BuildCartResponseAsync(cartId);
    }

    public async Task<CartResponse> AddToCartAsync(Guid userId, AddToCartRequest req)
    {
        ValidateQuantity(req.Quantity);

        var product = await GetAvailableProductAsync(req.ProductId);
        if (product is null)
            throw new InvalidOperationException("Product is not available or out of stock.");

        var cartId = await EnsureCartIdAsync(userId);
        await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        var existingQuantity = await _db.CartItems
            .AsNoTracking()
            .Where(i => i.CartId == cartId && i.ProductId == req.ProductId && !i.IsDeleted)
            .Select(i => (int?)i.Quantity)
            .FirstOrDefaultAsync();

        var nextQuantity = (existingQuantity ?? 0) + req.Quantity;
        if (nextQuantity > product.StockQuantity)
            throw new InvalidOperationException("Not enough stock available.");

        if (existingQuantity.HasValue)
        {
            await _db.CartItems
                .Where(i => i.CartId == cartId && i.ProductId == req.ProductId && !i.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(i => i.Quantity, nextQuantity)
                    .SetProperty(i => i.UpdatedAt, DateTime.UtcNow));
        }
        else
        {
            _db.CartItems.Add(new CartItem
            {
                CartId = cartId,
                ProductId = req.ProductId,
                Quantity = req.Quantity
            });

            await _db.SaveChangesAsync();
        }

        await UpdateCartTotalAsync(cartId);
        await transaction.CommitAsync();
        return await BuildCartResponseAsync(cartId);
    }

    public async Task<CartResponse> UpdateQuantityAsync(Guid userId, Guid itemId, UpdateCartItemRequest req)
    {
        ValidateQuantity(req.Quantity);

        var cartId = await EnsureCartIdAsync(userId);
        var item = await _db.CartItems
            .AsNoTracking()
            .Where(i => i.Id == itemId && i.CartId == cartId && !i.IsDeleted)
            .Select(i => new { i.Id, i.ProductId })
            .FirstOrDefaultAsync();

        if (item is null)
            throw new KeyNotFoundException("Item not found in cart.");

        var product = await GetAvailableProductAsync(item.ProductId);
        if (product is null)
            throw new InvalidOperationException("Product is no longer available.");

        if (req.Quantity > product.StockQuantity)
            throw new InvalidOperationException("Requested quantity exceeds available stock.");

        await using var transaction = await _db.Database.BeginTransactionAsync();

        var affectedRows = await _db.CartItems
            .Where(i => i.Id == itemId && i.CartId == cartId && !i.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(i => i.Quantity, req.Quantity)
                .SetProperty(i => i.UpdatedAt, DateTime.UtcNow));

        if (affectedRows == 0)
            throw new KeyNotFoundException("Item not found in cart.");

        await UpdateCartTotalAsync(cartId);
        await transaction.CommitAsync();
        return await BuildCartResponseAsync(cartId);
    }

    public async Task<CartResponse> RemoveItemAsync(Guid userId, Guid itemId)
    {
        var cartId = await EnsureCartIdAsync(userId);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        await _db.CartItems
            .Where(i => i.Id == itemId && i.CartId == cartId && !i.IsDeleted)
            .ExecuteDeleteAsync();

        await UpdateCartTotalAsync(cartId);
        await transaction.CommitAsync();
        return await BuildCartResponseAsync(cartId);
    }

    public async Task<CartResponse> ClearCartAsync(Guid userId)
    {
        var cartId = await EnsureCartIdAsync(userId);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        await _db.CartItems
            .Where(i => i.CartId == cartId && !i.IsDeleted)
            .ExecuteDeleteAsync();

        await UpdateCartTotalAsync(cartId);
        await transaction.CommitAsync();
        return await BuildCartResponseAsync(cartId);
    }

    private async Task<Guid> EnsureCartIdAsync(Guid userId)
    {
        var existingCart = await _db.Carts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new { c.Id, c.IsDeleted })
            .FirstOrDefaultAsync();

        if (existingCart is not null)
        {
            if (existingCart.IsDeleted)
            {
                await _db.Carts
                    .IgnoreQueryFilters()
                    .Where(c => c.Id == existingCart.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(c => c.IsDeleted, false)
                        .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));
            }

            return existingCart.Id;
        }

        var cart = new Cart
        {
            UserId = userId,
            TotalPrice = 0m
        };

        _db.Carts.Add(cart);

        try
        {
            await _db.SaveChangesAsync();
            return cart.Id;
        }
        catch (DbUpdateException)
        {
            _db.Entry(cart).State = EntityState.Detached;

            var concurrentCart = await _db.Carts
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .Select(c => new { c.Id, c.IsDeleted })
                .FirstOrDefaultAsync();

            if (concurrentCart is null)
                throw;

            if (concurrentCart.IsDeleted)
            {
                await _db.Carts
                    .IgnoreQueryFilters()
                    .Where(c => c.Id == concurrentCart.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(c => c.IsDeleted, false)
                        .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));
            }

            return concurrentCart.Id;
        }
    }

    private Task<Product?> GetAvailableProductAsync(Guid productId)
    {
        return _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsAvailable);
    }

    private async Task UpdateCartTotalAsync(Guid cartId)
    {
        var total = await _db.CartItems
            .AsNoTracking()
            .Where(i => i.CartId == cartId && !i.IsDeleted)
            .Join(
                _db.Products.AsNoTracking(),
                item => item.ProductId,
                product => product.Id,
                (item, product) => (decimal?)(item.Quantity * product.Price))
            .SumAsync() ?? 0m;

        await _db.Carts
            .IgnoreQueryFilters()
            .Where(c => c.Id == cartId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.TotalPrice, total)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow)
                .SetProperty(c => c.IsDeleted, false));
    }

    private async Task<CartResponse> BuildCartResponseAsync(Guid cartId)
    {
        var cartExists = await _db.Carts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(c => c.Id == cartId);

        if (!cartExists)
            throw new KeyNotFoundException("Cart not found.");

        var items = await _db.CartItems
            .AsNoTracking()
            .Where(i => i.CartId == cartId && !i.IsDeleted)
            .Join(
                _db.Products.AsNoTracking(),
                item => item.ProductId,
                product => product.Id,
                (item, product) => new CartItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductTitle = product.Title,
                    ProductImageUrl = product.ImageUrl,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity
                })
            .ToListAsync();

        return new CartResponse
        {
            Id = cartId,
            Items = items,
            TotalPrice = items.Sum(i => i.TotalPrice)
        };
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");
    }
}
