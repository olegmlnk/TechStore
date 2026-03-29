using System.ComponentModel.DataAnnotations;

namespace TechStore.API.Models;

public class OrderItem : BaseModel
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal UnitPrice { get; set; }
}
