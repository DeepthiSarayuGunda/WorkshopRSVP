using System.ComponentModel.DataAnnotations;

namespace WorkshopRSVP.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? BannerUrl { get; set; }

        // The Identity UserId of the Organizer who created this event
        public string? OrganizerUserId { get; set; }

        public List<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}
