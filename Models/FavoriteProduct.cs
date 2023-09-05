using System.ComponentModel.DataAnnotations.Schema;

namespace CultureWeb.Models
{
    public class FavoriteProduct
    {
        public int Id { get; set; } // Unique identifier for the favorite product
        public string UserId { get; set; } // User's identifier who favorited the product
        public int ProductId { get; set; } // Identifier of the product that was favorited
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; } // Navigation property
        public DateTime FavoritedAt { get; set; } // Timestamp when the product was favorited
                                                  // You can add more properties as needed, such as references to the actual Product model

    }
}
