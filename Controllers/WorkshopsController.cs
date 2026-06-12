using Microsoft.AspNetCore.Mvc;
using WorkshopRSVP.Models;

namespace WorkshopRSVP.Controllers
{
    public class WorkshopsController : Controller
    {
        private static List<Rsvp> _registrations = new List<Rsvp>();

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
            _registrations.Add(model);
            ViewData["Message"] = $"Thanks for registering, {model.FullName}!";
            return View(model);
        }

        public IActionResult Registrations()
        {
            return View(_registrations);
        }
    }
}
