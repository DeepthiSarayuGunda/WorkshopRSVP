using WorkshopRSVP.Models;

namespace WorkshopRSVP.Data
{
    public static class DbInitializer
    {
        public static void Initialize(EventManagerContext context)
        {
            context.Database.EnsureCreated();

            // don't seed if data already exists
            if (context.Events.Any())
                return;

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
