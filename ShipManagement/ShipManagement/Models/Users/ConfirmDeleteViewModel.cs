using Microsoft.AspNetCore.Identity;
using ShipManagement.Models.Tasks;

namespace ShipManagement.Models.Users;

public class ConfirmDeleteViewModel
{
    public IdentityUser User { get; set; }
    public IEnumerable<TaskViewModel> Tasks { get; set; }
}