namespace ShipManagement.Models.Users
{
    public class UserRoleInfo
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}