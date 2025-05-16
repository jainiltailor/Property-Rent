using Microsoft.AspNetCore.Mvc;
using PropertyRent.Data;
using System.Linq;
using PropertyRent.Models;

namespace PropertyRent.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var pending = _context.Properties.Where(p => !p.IsApproved).ToList();
            return View(pending);
        }

        public IActionResult Approve(int id)
        {
            var property = _context.Properties.Find(id);
            property.IsApproved = true;
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public IActionResult BookingHistory()
        {
            var bookings = _context.Bookings.ToList();
            return View(bookings);
        }
    }
}