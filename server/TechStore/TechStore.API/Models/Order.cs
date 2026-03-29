using System.ComponentModel.DataAnnotations;

namespace TechStore.API.Models;

public class Order : BaseModel
{
    public Guid UserId { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
