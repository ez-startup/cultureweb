using System.ComponentModel.DataAnnotations;

namespace CultureWeb.Models
{
    public class Review
    {
        public int Id { get; set; }
     
        [Required(ErrorMessage = "Please enter a review title text.")]
        public string ReviewTitle { get; set; }
        [Required(ErrorMessage = "Please enter a rate of product.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please enter a review text.")]
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsApproved { get; set; } // Indicates whether the review is approved by admin
        public int ProductId { get; set; } // Foreign key to associate the review with a product
        public virtual Products Product { get; set; }

        // Foreign key to associate the review with the user who submitted it
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
