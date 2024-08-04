using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShipManagement.Models;

namespace ShipManagement.Data;

public partial class ShipManagementDbContext : DbContext
{
    public ShipManagementDbContext()
    {
    }

    public ShipManagementDbContext(DbContextOptions<ShipManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Server=tcp:shipmanagementdatabaseserver.database.windows.net,1433;Initial Catalog=Shipmanagementdatabase;Persist Security Info=False;User ID=kvatevadmin;Password=\"P@ssword123\";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
