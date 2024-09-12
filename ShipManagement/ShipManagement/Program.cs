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
        builder.Configuration.AddEnvironmentVariables();
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ShipManagementDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ShipManagementDbContext>();
        
        builder.Services.AddControllersWithViews();
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("CanAssignTasks", policy =>
                policy.RequireRole("Администратор","Адмирал", "Вицеадмирал","Контраадмирал", "Флотилен адмирал"));
        });

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
                "Администратор","Адмирал", "Вицеадмирал", "Контраадмирал", "Флотилен адмирал", "Капитан I ранг", "Капитан II ранг", "Капитан III ранг",
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