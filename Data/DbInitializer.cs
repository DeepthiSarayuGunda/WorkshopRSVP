using Microsoft.AspNetCore.Identity;
using WorkshopRSVP.Models;

namespace WorkshopRSVP.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<EventManagerContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // drop and recreate to get Identity tables
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // seed roles
            if (!await roleManager.RoleExistsAsync("Organizer"))
                await roleManager.CreateAsync(new IdentityRole("Organizer"));

            if (!await roleManager.RoleExistsAsync("Attendee"))
                await roleManager.CreateAsync(new IdentityRole("Attendee"));

            // seed organizer user
            if (await userManager.FindByEmailAsync("organizer@example.com") == null)
            {
                var organizer = new IdentityUser
                {
                    UserName = "organizer@example.com",
                    Email = "organizer@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(organizer, "Organizer123!");
                await userManager.AddToRoleAsync(organizer, "Organizer");
            }

            // seed attendee user
            if (await userManager.FindByEmailAsync("attendee@example.com") == null)
            {
                var attendee = new IdentityUser
                {
                    UserName = "attendee@example.com",
                    Email = "attendee@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(attendee, "Attendee123!");
                await userManager.AddToRoleAsync(attendee, "Attendee");
            }

            // seed events if none exist
            if (!context.Events.Any())
            {
                var events = new Event[]
                {
                    new Event
                    {
                        Title = "Routing Workshop",
                        Description = "A hands-on workshop covering ASP.NET Core routing fundamentals.",
                        Date = new DateTime(2026, 3, 17, 5, 56, 0),
                        Location = "Algonquin College - T Building",
                        Attendees = new List<Attendee>
                        {
                            new Attendee { Name = "Alice Smith", Email = "alice@example.com" },
                            new Attendee { Name = "Bob Jones", Email = "bob@example.com" }
                        }
                    },
                    new Event
                    {
                        Title = "Tech Conference 2026",
                        Description = "A full-day conference about cloud, AI, and enterprise apps.",
                        Date = new DateTime(2026, 3, 27, 5, 56, 0),
                        Location = "Ottawa Convention Centre",
                        Attendees = new List<Attendee>
                        {
                            new Attendee { Name = "Charlie Brown", Email = "charlie@example.com" },
                            new Attendee { Name = "Diana Prince", Email = "diana@example.com" }
                        }
                    },
                    new Event
                    {
                        Title = "EF Core Bootcamp",
                        Description = "Learn Entity Framework Core from scratch with real-world examples.",
                        Date = new DateTime(2026, 4, 6, 5, 56, 0),
                        Location = "Online",
                        Attendees = new List<Attendee>
                        {
                            new Attendee { Name = "Eve Wilson", Email = "eve@example.com" },
                            new Attendee { Name = "Frank Miller", Email = "frank@example.com" }
                        }
                    }
                };

                context.Events.AddRange(events);
                context.SaveChanges();
            }
        }
    }
}
