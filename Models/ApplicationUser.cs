using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CultureWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoinDate { get; set; }

        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Navigation property to represent the relationship between users and their reviews
        public virtual List<Review> Reviews { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
    }
}
