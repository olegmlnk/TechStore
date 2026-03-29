using TechStore.Core.Enums;

namespace TechStore.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
    public Category Category { get; set; }
    public string CategoryName => Category.ToString();
    public string ImageUrl { get; set; } = string.Empty;
}

public class ProductListDto
{
    public IEnumerable<ProductDto> Products { get; set; } = [];
    public int TotalCount { get; set; }
}
