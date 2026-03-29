using System.ComponentModel.DataAnnotations;
using TechStore.Core.Enums;

namespace TechStore.Core.Models;

public class Product : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public Category Category { get; set; }

    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
}
