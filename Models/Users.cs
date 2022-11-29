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

        [Required(ErrorMessage = "User's Name is Required")]
        public string? FullName { get; set; }
        
        [Required]
        [MinLength(8, ErrorMessage = "Password must be longer than 8 characters long")]
        [MaxLength(30, ErrorMessage = "Password cannot be longer than 30 characters long")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8}$", 
            ErrorMessage = "Password must have at least one small letter, one upper case, one special character, and one number"
            )]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string Active { get; set; }
    }
}
