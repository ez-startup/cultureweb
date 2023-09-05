using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CultureWeb.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string Title_kh { get; set; }
        public string? Image { get; set; }
      
        [Display(Name = "Sub Category")]
        [Required]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory? SubCategories { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Description_kh { get; set; }

        public string? BlogTitle { get; set; }
        public string? BlogDesc { get; set; }
        public string? BlogTitle_kh { get; set; }       
        public string? BlogDesc_kh { get; set; }

    }
}
