using System.ComponentModel.DataAnnotations;

namespace WorkshopRSVP.Models
{
    public class Rsvp
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public bool NeedsAccommodation { get; set; }

        [Required]
        public string WorkshopTitle { get; set; } = string.Empty;
    }
}
