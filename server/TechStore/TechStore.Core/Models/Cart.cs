using System.ComponentModel.DataAnnotations;

namespace TechStore.Core.Models;

public class Cart : BaseModel
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal TotalPrice { get; set; }
}
