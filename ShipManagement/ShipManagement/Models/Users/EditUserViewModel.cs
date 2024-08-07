using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models.UserManagement
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}