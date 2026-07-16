using Microsoft.EntityFrameworkCore;
using WorkshopRSVP.Models;

namespace WorkshopRSVP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rsvp> Rsvps { get; set; }
    }
}
