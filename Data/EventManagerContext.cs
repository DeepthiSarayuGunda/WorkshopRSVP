using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopRSVP.Models;

namespace WorkshopRSVP.Data
{
    public class EventManagerContext : IdentityDbContext<IdentityUser>
    {
        public EventManagerContext(DbContextOptions<EventManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // cascade delete so attendees get removed when event is deleted
            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.Event)
                .WithMany(e => e.Attendees)
                .HasForeignKey(a => a.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // index on Attendee.UserId + EventId to enforce unique registration per user per event
            modelBuilder.Entity<Attendee>()
                .HasIndex(a => new { a.UserId, a.EventId })
                .IsUnique()
                .HasFilter("[UserId] IS NOT NULL");
        }
    }
}
