using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models.Users
{
    public class RegisterViewModel
    {
        public string PhoneNumber { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        
        public string Password { get; set; }
    }
}