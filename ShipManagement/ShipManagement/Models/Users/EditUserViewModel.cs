using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        
        public string PhoneNumber { get; set; }
        
        [Required]
        public string Username { get; set; }
    }
}