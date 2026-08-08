using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkshopRSVP.Data;
using WorkshopRSVP.Hubs;
using WorkshopRSVP.Models;

namespace WorkshopRSVP.Controllers
{
    [Route("events/{eventId}/attendees")]
    public class AttendeesController : Controller
    {
        private readonly EventManagerContext _context;
        private readonly IHubContext<EventHub> _hubContext;

        public AttendeesController(EventManagerContext context, IHubContext<EventHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // any logged in user can view attendees
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Index(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
                return NotFound();

            ViewBag.Event = ev;
            return View(ev.Attendees);
        }

        // Self-registration: any authenticated user can register themselves
        [Authorize]
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Unknown";
            var userName = User.Identity?.Name ?? userEmail;

            // Check if user is already registered for this event
            var alreadyRegistered = await _context.Attendees
                .AnyAsync(a => a.EventId == eventId && a.UserId == userId);

            if (alreadyRegistered)
            {
                TempData["Error"] = "You are already registered for this event.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var attendee = new Attendee
            {
                Name = userName,
                Email = userEmail,
                EventId = eventId,
                UserId = userId
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            // SignalR: broadcast updated attendee count and new attendee name to all clients viewing this event
            var attendeeCount = await _context.Attendees.CountAsync(a => a.EventId == eventId);
            await _hubContext.Clients.Group($"event-{eventId}")
                .SendAsync("ReceiveAttendeeUpdate", attendeeCount, attendee.Name, attendee.Email);

            // SignalR: send private notification to the event's Organizer
            if (!string.IsNullOrEmpty(ev.OrganizerUserId))
            {
                var message = $"{userEmail} just registered for your {ev.Title}.";
                await _hubContext.Clients.User(ev.OrganizerUserId)
                    .SendAsync("ReceiveOrganizerNotification", message);
            }

            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // Self-unregister: any authenticated user can remove themselves
        [Authorize]
        [HttpPost("unregister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unregister(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId);

            if (attendee == null)
            {
                TempData["Error"] = "You are not registered for this event.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // only organizers can add attendees directly
        [Authorize(Roles = "Organizer")]
        [HttpGet("create")]
        public async Task<IActionResult> Create(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null)
                return NotFound();

            ViewBag.Event = ev;
            return View(new Attendee { EventId = eventId });
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, Attendee attendee)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null)
                return NotFound();

            attendee.EventId = eventId;
            ModelState.Remove("Event");

            if (ModelState.IsValid)
            {
                _context.Attendees.Add(attendee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { eventId });
            }

            ViewBag.Event = ev;
            return View(attendee);
        }

        [Authorize(Roles = "Organizer")]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int eventId, int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null || attendee.EventId != eventId)
                return NotFound();

            var ev = await _context.Events.FindAsync(eventId);
            ViewBag.Event = ev;
            return View(attendee);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int eventId, int id, Attendee attendee)
        {
            if (id != attendee.Id)
                return NotFound();

            attendee.EventId = eventId;
            ModelState.Remove("Event");

            if (ModelState.IsValid)
            {
                _context.Update(attendee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { eventId });
            }

            var ev = await _context.Events.FindAsync(eventId);
            ViewBag.Event = ev;
            return View(attendee);
        }

        // only organizers can remove attendees
        [Authorize(Roles = "Organizer")]
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int eventId, int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null || attendee.EventId != eventId)
                return NotFound();

            var ev = await _context.Events.FindAsync(eventId);
            ViewBag.Event = ev;
            return View(attendee);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventId, int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee != null && attendee.EventId == eventId)
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}
