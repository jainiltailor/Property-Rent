using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using PropertyRent.Models;
using PropertyRent.Data;

namespace PropertyRent.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PropertyController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(Property property, IFormFile imageFile)
        {
            if (HttpContext.Session.GetInt32("UserId") is int userId)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using var stream = new FileStream(filePath, FileMode.Create);
                    imageFile.CopyTo(stream);
                    property.ImagePath = "/uploads/" + fileName;
                }

                property.UserId = userId;
                property.IsApproved = false;
                _context.Properties.Add(property);
                _context.SaveChanges();
                //return RedirectToAction("MyProperties");
            }
            return RedirectToAction("Dashboard", "User");
        }
        [HttpGet]
        public IActionResult MyProperties()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var list = _context.Properties.Where(p => p.UserId == userId).ToList();
            return RedirectToAction("Dashboard", "User");
        }

        public IActionResult List()
        {
            var list = _context.Properties.Where(p => p.IsApproved).ToList();
            return View(list);
        }

        public IActionResult Search(string query, int? minPrice, int? maxPrice)
        {
            var results = _context.Properties.Where(p => p.IsApproved);

            if (!string.IsNullOrEmpty(query))
            {
                results = results.Where(p => p.Title.Contains(query));
            }

            if (minPrice.HasValue)
            {
                results = results.Where(p => p.PricePerDay >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                results = results.Where(p => p.PricePerDay <= maxPrice);
            }

            ViewBag.Query = query;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View("Search", results.ToList());
        }
    }
}