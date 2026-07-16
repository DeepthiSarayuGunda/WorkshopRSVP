using System.ComponentModel.DataAnnotations;

namespace WorkshopRSVP.Models
{
    public class Attendee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int EventId { get; set; }

        public Event? Event { get; set; }
    }
}
