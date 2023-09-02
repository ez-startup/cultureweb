using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace CultureWeb.Models
{
    public class ProductAttribute

    {
        public int Id { get; set; }

        [Required]
        [Remote("CheckedProductExist", "Products", ErrorMessage = "Product doesn't exist.")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [DataType(DataType.Text)]
        public virtual Products Product { get; set; }

        [Required]
        [Remote("CheckedAttributeExist", "Attributes", ErrorMessage = "Attribute doesn't exist.")]
        public int AttributeId { get; set; }

        [ForeignKey("AttributeId")]
        [DataType(DataType.Text)]
        public virtual Attribute Attribute { get; set; }
    }
}
