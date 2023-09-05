using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CultureWeb.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sub Category")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public string Name_kh { get; set; }
        public string? Description_kh { get; set; }
        public string? Image { get; set; }
        [Required]
        public int MainCategoryId { get; set; }
        [ForeignKey("MainCategoryId")]
        public virtual MainCategory? MainCategories { get; set; }

        public virtual List<Blog> Blogs { get; set; }
        public virtual List<Products> Products { get; set; }


    }
}
