using Microsoft.EntityFrameworkCore;
using TechStore.Application.DTOs;
using TechStore.Core.Enums;
using TechStore.Core.Models;
using TechStore.Infrastructure.Shared;

namespace TechStore.Application.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProductListDto> GetAllAsync()
    {
        var products = await _db.Products
            .Where(p => p.IsAvailable)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return new ProductListDto { Products = products, TotalCount = products.Count };
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductListDto> GetByCategoryAsync(Category category)
    {
        var products = await _db.Products
            .Where(p => p.IsAvailable && p.Category == category)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return new ProductListDto { Products = products, TotalCount = products.Count };
    }

    public async Task<ProductListDto> SearchAsync(string query)
    {
        var lower = query.ToLower();
        var products = await _db.Products
            .Where(p => p.IsAvailable &&
                (p.Title.ToLower().Contains(lower) || p.Description.ToLower().Contains(lower)))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return new ProductListDto { Products = products, TotalCount = products.Count };
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Price = p.Price,
        IsAvailable = p.IsAvailable,
        StockQuantity = p.StockQuantity,
        Category = p.Category,
        ImageUrl = p.ImageUrl
    };

    // ── Admin methods ────────────────────────────────────────

    public async Task<ProductListDto> GetAllForAdminAsync()
    {
        var products = await _db.Products
            .IgnoreQueryFilters()
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return new ProductListDto { Products = products, TotalCount = products.Count };
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            IsAvailable = request.IsAvailable,
            StockQuantity = request.StockQuantity,
            Category = request.Category,
            ImageUrl = request.ImageUrl
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return MapToDto(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        product.Title = request.Title;
        product.Description = request.Description;
        product.Price = request.Price;
        product.IsAvailable = request.IsAvailable;
        product.StockQuantity = request.StockQuantity;
        product.Category = request.Category;
        product.ImageUrl = request.ImageUrl;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
