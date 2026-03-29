namespace TechStore.API.Models;

public class Wishlist : BaseModel
{
    public Guid UserId { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
