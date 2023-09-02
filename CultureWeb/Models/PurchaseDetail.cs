using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CultureWeb.Models
{
    public class PurchaseDetail
    {

        public int Id { get; set; }
        [Display(Name = "Purchase")]
        public int PurchaseId { get; set; }
        [Display(Name = "Product")]
        public int PorductId { get; set; }

        [ForeignKey("PurchaseId")]
        public Purchase Purchase { get; set; }

        [ForeignKey("PorductId")]
        public Products Product { get; set; }

        public int QtyPurchase { get; set; }
        public decimal CostPrice { get; set; }
    }
}
