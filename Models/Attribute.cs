using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Localization;

namespace CultureWeb.Models
{
    public class Attribute
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Attribute's Name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Attribute's Material is required.")]
        public string? Meterial { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Attribute's Name in Khmer is required.")]
        public string Name_kh { get; set; }

        [Required(ErrorMessage = "Attribute's Material in Khmer is required.")]
        public string? Meterial_kh { get; set; }
        public string? Description_kh { get; set; }
        public ICollection<ProductAttribute> ProductAttributes { get; set; }
    }
}
