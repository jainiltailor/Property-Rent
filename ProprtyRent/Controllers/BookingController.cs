using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PropertyRent.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using PropertyRent.Models;
using Azure.Messaging;

namespace PropertyRent.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Book(int id) => View(_context.Properties.Find(id));

        [HttpPost]
        public IActionResult Book(int propertyId, DateTime startDate, DateTime endDate, int numberOfPeople)
        {
                
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            var existingBookings = _context.Bookings
            .Where(b => b.PropertyId == propertyId).ToList();

            // Check for overlap
            bool isConflict = existingBookings.Any(b =>
                startDate < b.EndDate && endDate > b.StartDate);

            if (isConflict)
            {
                return RedirectToAction("List", "Property");
            }
            else
            {

                var property = _context.Properties.Find(propertyId);


            if (property != null)
            {
            
                var totalDays = (endDate - startDate).Days;
                var totalAmount = totalDays * property.PricePerDay;

            var booking = new Booking()
            {
                Name= userId.Value.ToString(),
                PropertyId = propertyId,
                UserId = userId.Value,
                StartDate = startDate,
                EndDate = endDate,
                NumberOfPeople = numberOfPeople,
                TotalAmount = totalAmount
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            return RedirectToAction("Receipt", new { id = booking.Id });
            }
            else
            {
                return RedirectToAction("Landing");
            }
            }
        }

        public IActionResult Receipt(int id)
        {
            var booking = _context.Bookings
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.NumberOfPeople,
                    b.StartDate,
                    b.EndDate,
                    b.TotalAmount
                })
                .FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Booking ID: {booking.Id}");
            sb.AppendLine($"Name: {booking.Name}");
            sb.AppendLine($"People: {booking.NumberOfPeople}");
            sb.AppendLine($"Duration: {booking.StartDate:dd-MM-yyyy} to {booking.EndDate:dd-MM-yyyy}");
            sb.AppendLine($"Total Amount: ₹{booking.TotalAmount}");

            var fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "text/plain", $"Booking_{booking.Id}.txt");
        }

        public IActionResult MyBookings()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .ToList();
            return View(bookings);
        }
    }
}
