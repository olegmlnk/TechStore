using System.ComponentModel.DataAnnotations;

namespace TechStore.Core.Models;

public class Address : BaseModel
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;
}
