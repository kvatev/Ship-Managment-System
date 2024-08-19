using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShipManagement.Models.Tasks;

namespace ShipManagement.Data;

public class ShipManagementDbContext : IdentityDbContext
{
    public ShipManagementDbContext(DbContextOptions<ShipManagementDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<TaskViewModel> Tasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TaskViewModel>()
            .HasOne(t => t.AssignedBy) // Updated to AssignedByUser
            .WithMany()
            .HasForeignKey(t => t.AssignedById) // Updated to AssignedByUserId
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaskViewModel>()
            .HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}