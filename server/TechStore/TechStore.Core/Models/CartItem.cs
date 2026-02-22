using System.ComponentModel.DataAnnotations;

namespace TechStore.Core.Models;

public class CartItem : BaseModel
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
