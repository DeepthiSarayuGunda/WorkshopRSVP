using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            // Ensure database schema is created
            // If DB already exists, try adding missing columns for Assignment 3
            var created = await context.Database.EnsureCreatedAsync();
            if (!created)
            {
                // Database already exists - add missing columns if needed
                try
                {
                    await context.Database.ExecuteSqlRawAsync(
                        @"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Attendees') AND name = 'UserId')
                          ALTER TABLE Attendees ADD UserId NVARCHAR(MAX) NULL;");
                    await context.Database.ExecuteSqlRawAsync(
                        @"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Events') AND name = 'OrganizerUserId')
                          ALTER TABLE Events ADD OrganizerUserId NVARCHAR(MAX) NULL;");
                }
                catch { /* columns may already exist */ }
            }

            // seed roles
            if (!await roleManager.RoleExistsAsync("Organizer"))
                await roleManager.CreateAsync(new IdentityRole("Organizer"));

            if (!await roleManager.RoleExistsAsync("Attendee"))
                await roleManager.CreateAsync(new IdentityRole("Attendee"));

            // seed organizer user
            IdentityUser? organizerUser = null;
            if (await userManager.FindByEmailAsync("organizer@example.com") == null)
            {
                organizerUser = new IdentityUser
                {
                    UserName = "organizer@example.com",
                    Email = "organizer@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(organizerUser, "Organizer123!");
                await userManager.AddToRoleAsync(organizerUser, "Organizer");
            }
            else
            {
                organizerUser = await userManager.FindByEmailAsync("organizer@example.com");
            }

            // seed attendee user
            IdentityUser? attendeeUser = null;
            if (await userManager.FindByEmailAsync("attendee@example.com") == null)
            {
                attendeeUser = new IdentityUser
                {
                    UserName = "attendee@example.com",
                    Email = "attendee@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(attendeeUser, "Attendee123!");
                await userManager.AddToRoleAsync(attendeeUser, "Attendee");
            }
            else
            {
                attendeeUser = await userManager.FindByEmailAsync("attendee@example.com");
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
                        OrganizerUserId = organizerUser!.Id,
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
                        OrganizerUserId = organizerUser.Id,
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
                        OrganizerUserId = organizerUser.Id,
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
