namespace WorkshopRSVP.Models
{
    public class Rsvp
    {
        public string FullName { get; set; } = string.Empty;
        public bool NeedsAccommodation { get; set; }
        public string WorkshopTitle { get; set; } = string.Empty;
    }
}
