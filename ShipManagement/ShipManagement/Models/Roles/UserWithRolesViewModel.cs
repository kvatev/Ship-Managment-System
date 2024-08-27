using Microsoft.AspNetCore.Identity;

namespace ShipManagement.Models.Roles;

public class UserWithRolesViewModel
{
    public IdentityUser User { get; set; }
    public IList<string> Roles { get; set; }
}