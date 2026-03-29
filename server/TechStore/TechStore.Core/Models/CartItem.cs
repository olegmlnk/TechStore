using System.ComponentModel.DataAnnotations;

namespace TechStore.Core.Models;

public class CartItem : BaseModel
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
