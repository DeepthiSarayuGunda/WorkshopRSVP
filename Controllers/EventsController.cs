using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkshopRSVP.Data;
using WorkshopRSVP.Models;
using WorkshopRSVP.Services;

namespace WorkshopRSVP.Controllers
{
    [Route("events")]
    public class EventsController : Controller
    {
        private readonly EventManagerContext _context;
        private readonly IBlobService _blobService;

        public EventsController(EventManagerContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // anyone can browse events
        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        [AllowAnonymous]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // only organizers can create events
        [Authorize(Roles = "Organizer")]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event ev, IFormFile? bannerImage)
        {
            ModelState.Remove("bannerImage");

            if (ModelState.IsValid)
            {
                if (bannerImage != null && bannerImage.Length > 0)
                {
                    ev.BannerUrl = await _blobService.UploadFileAsync(bannerImage);
                }

                // Store the current user's Id as the event organizer
                ev.OrganizerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Events.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        // only organizers can edit
        [Authorize(Roles = "Organizer")]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event ev, IFormFile? bannerImage)
        {
            if (id != ev.Id)
                return NotFound();

            ModelState.Remove("bannerImage");

            if (ModelState.IsValid)
            {
                if (bannerImage != null && bannerImage.Length > 0)
                {
                    ev.BannerUrl = await _blobService.UploadFileAsync(bannerImage);
                }
                else
                {
                    var existing = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (existing != null)
                        ev.BannerUrl = existing.BannerUrl;
                }

                _context.Update(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        // only organizers can delete
        [Authorize(Roles = "Organizer")]
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
