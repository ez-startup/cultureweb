using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CultureWeb.Models
{
    public class Products
    {

        public Products()
        {
            OrderDetails = new List<OrderDetails>();
            PurchaseDetails = new List<PurchaseDetail>();
            Reviews = new List<Review>(); // Initialize the Reviews property
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Product's Name is required.")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Product's Name in Khmer is required.")]
        public string Name_kh { get; set; }
        public string? Description_kh { get; set; }

        [Required(ErrorMessage = "Product's Price is required.")]
        public decimal Price { get; set; }
        public string? Image { get; set; }

        [Required(ErrorMessage = "Product's Color is required.")]
        [Display(Name = "Product Color")]
        public string? ProductColor { get; set; }

        [Required(ErrorMessage = "Product's Color is required.")]
        public string? ProductColor_kh { get; set; }

        [Required]
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Sub Category")]

        [Required(ErrorMessage = "Product's sub category is required.")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory? SubCategories { get; set; }
              
        public string? Information { get; set; }

        [Required(ErrorMessage = "Product's quantity is required.")]
        public int? Qty { get; set; }

        public virtual List<OrderDetails> OrderDetails { get; set; }
        public virtual List<PurchaseDetail> PurchaseDetails { get; set; }

        public ICollection<ProductAttribute> ProductAttributes { get; set; }

        // New property to represent the reviews associated with the product
        public virtual List<Review> Reviews { get; set; }


    }
}
