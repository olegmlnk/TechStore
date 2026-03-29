namespace TechStore.Core.Models;

public class Wishlist : BaseModel
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}
