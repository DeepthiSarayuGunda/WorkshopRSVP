using Microsoft.AspNetCore.Mvc;
using WorkshopRSVP.Data;
using WorkshopRSVP.Models;

namespace WorkshopRSVP.Controllers
{
    public class WorkshopsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkshopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RsvpForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Confirm(Rsvp model)
        {
            ViewData["Message"] = $"Thanks for registering, {model.FullName}!";
            return View(model);
        }

        public IActionResult Registrations()
        {
            var registrations = _context.Rsvps.ToList();
            return View(registrations);
        }
    }
}
