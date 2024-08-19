using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShipManagement.Data;

public class ShipManagementDbContext : IdentityDbContext
{
    public ShipManagementDbContext(DbContextOptions<ShipManagementDbContext> options)
        : base(options)
    {
    }
}