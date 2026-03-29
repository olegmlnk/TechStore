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
}
