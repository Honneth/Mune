using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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


            // Clear database
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // DEVELOPMENT ONLY
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

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

            // Sampler users and posts
            using (var scope = app.Services.CreateScope())
            {
                // ___ Sample users ___
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                for (int i = 1; i <= 3; i++)
                {
                    var email = $"Bruger{i}@Bruger{i}.dk";
                    var password = $"!Bruger{i}";

                    // If no such user exists
                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        // Create new user
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

                        // For first created user create post
                        if (i == 1)
                        {
                            context.UserPosts.Add(new UserPost
                            {
                                UserId = user.Id,
                                Headline = $"Guitarrist søges",
                                City = "Odense",
                                PostText = $"Rockband i Odense søger en rock-guitarrist til at spille jobs.",
                                Timestamp = DateTime.Now
                            });

                            for (int j = 1; j < 10; j++)
                            {
                                context.UserPosts.Add(new UserPost
                                {
                                    UserId = user.Id,
                                    Headline = $"Opslag {j+1}",
                                    City = "Odense",
                                    PostText = $"Opslagstekst {j+1}",
                                    Timestamp = DateTime.Now
                                });
                            }


                            // For second created user create post
                        }
                        else if (i == 2)
                        {
                            context.UserPosts.Add(new UserPost
                            {
                                UserId = user.Id,
                                Headline = $"Trommeslager søges Nordjylland",
                                City = "Skagen",
                                PostText = $"Rockband i Skagen søger en trommeslager til at spille jobs.",
                                Timestamp = DateTime.Now
                            });

                            for (int j = 1; j < 10; j++)
                            {
                                context.UserPosts.Add(new UserPost
                                {
                                    UserId = user.Id,
                                    Headline = $"Opslag {j + 1}",
                                    City = "Odense",
                                    PostText = $"Opslagstekst {j + 1}",
                                    Timestamp = DateTime.Now
                                });
                            }
                        }
                    }
                }


                // Sample conversations
                var users = await context.Users.ToListAsync();

                // For each user i
                for (int i = 0; i < users.Count; i++)
                {
                    // for each other user
                    for (int j = i + 1; j < users.Count; j++)
                    {
                        var user1 = users[i];
                        var user2 = users[j];

                        // Check if conversation already exists
                        bool exists = await context.Conversations.AnyAsync(c =>
                            (c.User1Id == user1.Id && c.User2Id == user2.Id) ||
                            (c.User1Id == user2.Id && c.User2Id == user1.Id));

                        if (!exists)
                        {
                            var conversation = new Conversation
                            {
                                Timestamp = DateTime.Now,
                                User1Id = user1.Id,
                                User2Id = user2.Id
                            };

                            context.Conversations.Add(conversation);

                            // Save conversation
                            await context.SaveChangesAsync();

                            // Add messages
                            var message1 = new Message
                            {
                                ConversationId = conversation.Id,
                                SenderId = user1.Id,
                                ReceiverId = user2.Id,
                                MessageText = $"Hej {user2.UserName}!",
                                Timestamp = DateTime.Now
                            };

                            var message2 = new Message
                            {
                                ConversationId = conversation.Id,
                                SenderId = user2.Id,
                                ReceiverId = user1.Id,
                                MessageText = $"Hej {user1.UserName}!",
                                Timestamp = DateTime.Now.AddMinutes(1)
                            };

                            context.Messages.Add(message1);
                            context.Messages.Add(message2);

                            for (int k = 1; k < 10; k++)
                            {
                                // Add messages
                                var messagex = new Message
                                {
                                    ConversationId = conversation.Id,
                                    SenderId = user1.Id,
                                    ReceiverId = user2.Id,
                                    MessageText = $"Besked {k} til {user2.UserName}.",
                                    Timestamp = DateTime.Now
                                };

                                var messagez = new Message
                                {
                                    ConversationId = conversation.Id,
                                    SenderId = user2.Id,
                                    ReceiverId = user1.Id,
                                    MessageText = $"Besked {k} til {user1.UserName}.",
                                    Timestamp = DateTime.Now.AddMinutes(1)
                                };

                                context.Messages.Add(messagex);
                                context.Messages.Add(messagez);
                            }
                        }
                    }
                }

                await context.SaveChangesAsync();

            }
            app.Run();
        }
    }
}
