using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopRSVP.Data;
using WorkshopRSVP.Models;

namespace WorkshopRSVP.Controllers
{
    [Route("events/{eventId}/attendees")]
    public class AttendeesController : Controller
    {
        private readonly EventManagerContext _context;

        public AttendeesController(EventManagerContext context)
        {
            _context = context;
        }

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

        [HttpGet("create")]
        public async Task<IActionResult> Create(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null)
                return NotFound();

            ViewBag.Event = ev;
            return View(new Attendee { EventId = eventId });
        }

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
