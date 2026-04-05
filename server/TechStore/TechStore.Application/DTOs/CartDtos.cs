using System.ComponentModel.DataAnnotations;

namespace TechStore.Application.DTOs;

public class CartResponse
{
    public Guid Id { get; set; }
    public ICollection<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
    public decimal TotalPrice { get; set; }
}

public class CartItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

public class AddToCartRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    [Range(1, 100)]
    public int Quantity { get; set; }
}
