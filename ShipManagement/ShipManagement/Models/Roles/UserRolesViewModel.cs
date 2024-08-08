namespace ShipManagement.Models.Roles
{
    public class UserRolesViewModel
    {
        public string Id { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public IList<string> Roles { get; set; }
        
        public List<RoleViewModel> AllRoles { get; set; }
        
        public string SelectedRole { get; set; }
    }
}