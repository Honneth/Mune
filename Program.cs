using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mune.Data;
using Mune.Models;

namespace Mune
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Config of user administration. User inherits from IdentityUser
            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
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
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            // Declaring roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] {"User"};
                foreach (var role in roles)
                {
                    // hvis rollen ikke allerede eksisterer
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // Three test-users are created to make setup of system easier on different machines.
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                for (int i = 1; i <= 3; i++)
                {
                    var email = $"Bruger{i}@Bruger{i}.dk";
                    var password = $"!Bruger{i}";

                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        var user = new User();
                        user.Email = email;
                        user.UserName = $"Bruger{i}";
                        user.EmailConfirmed = true; // Hardcoded true, because email server is not set up in this prototype
                        var result = await userManager.CreateAsync(user, password);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                Console.WriteLine($"User creation error: {error.Description}");
                            }
                            continue;
                        }
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            app.Run();
        }
    }
}
