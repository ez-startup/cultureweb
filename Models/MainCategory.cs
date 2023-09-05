using Microsoft.Build.Framework;

namespace CultureWeb.Models
{
    public class MainCategory
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public string Name_kh { get; set; }
        public string? Description_kh { get; set; }
    }
}
