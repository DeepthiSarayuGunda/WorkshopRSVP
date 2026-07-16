using WorkshopRSVP.Models;

namespace WorkshopRSVP.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // don't seed if data already exists
            if (context.Rsvps.Any())
                return;

            var rsvps = new Rsvp[]
            {
                new Rsvp { FullName = "Alice Johnson", WorkshopTitle = "Routing Basics", NeedsAccommodation = true },
                new Rsvp { FullName = "Bob Smith", WorkshopTitle = "EF Core Intro", NeedsAccommodation = false },
                new Rsvp { FullName = "Charlie Lee", WorkshopTitle = "Razor Essentials", NeedsAccommodation = true },
                new Rsvp { FullName = "Diana Park", WorkshopTitle = "Routing Basics", NeedsAccommodation = false }
            };

            context.Rsvps.AddRange(rsvps);
            context.SaveChanges();
        }
    }
}
