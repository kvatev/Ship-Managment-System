using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShipManagement.Data;

namespace ShipManagement;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ShipManagementDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ShipManagementDbContext>();
        
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        using (var score = app.Services.CreateScope())
        {
            var roleManager = score.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            var roles = new[]
            {
                "Admiral","Адмирал", "Вицеадмирал", "Контраадмирал", "Флотилен адмирал", "Капитан ранг I", "Капитан ранг II", "Капитан ранг III",
                "Капитан-лейтенант", "Старши лейтенант", "Лейтенант", "Офицерски кандидат", "Мичман", "Главен старшина", "Старшина I степен", "Старшина II степен", "Старши матрос", "Матрос"
            };
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        app.Run();
    }
}