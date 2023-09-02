﻿using System.ComponentModel.DataAnnotations;

namespace CultureWeb.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]      
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime ContactDate { get; set; }

      
    }
}
