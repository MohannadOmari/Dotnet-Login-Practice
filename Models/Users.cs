using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace UmniahAssignment.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string? FullName { get; set; }
        
        [Required]
        [MinLength(8, ErrorMessage = "Password must be longer than 8 characters long")]
        [MaxLength(30, ErrorMessage = "Password cannot be longer than 30 characters long")]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string? Active { get; set; }
    }
}
