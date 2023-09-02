using System.ComponentModel.DataAnnotations;

namespace CultureWeb.Models
{
    public class Order
    {

        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }
        public int Id { get; set; }
        [Required]
        [Display(Name = "Order No")]
        public string OrderNo { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string Address { get; set; }

        public DateTime OrderDate { get; set; }

        public virtual List<OrderDetails> OrderDetails { get; set; }

        public string UserId { get; set; } // User's identifier who purchased the product
        public virtual ApplicationUser User { get; set; }
    }
}
