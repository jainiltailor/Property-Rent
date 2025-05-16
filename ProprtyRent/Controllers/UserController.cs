using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRent.Data;
using PropertyRent.Models;
using System.Security.Claims;

namespace PropertyRent.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "User")]
        public async Task<IActionResult> Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var myProps = _context.Properties.Where(p => p.Id == userId).ToList();
            var myBookings = _context.Bookings.Include(b => b.Property)
                                              .Where(b => b.UserId == userId).ToList();

            var vm = new UserDashboardViewModel
            {
                MyProperties = myProps,
                MyBookings = myBookings
            };

            return View(vm);
        }


        // Show Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handle Register Form Post
        [HttpPost]
        public IActionResult Register(User user)
        {
            
                // Check if user with email already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(user);
                }
                user.Role = "User";
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login","User");
            
        }

        // Show Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User loginUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginUser.Email && u.Password == loginUser.Password);

            if (user != null)
            {
            
                if (user.Role == "User")
                {
                    var claims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, "User") // Match this with [Authorize(Roles = "User")]
                    };

                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                HttpContext.Session.SetInt32("UserId", user.Id); // Save session

                return RedirectToAction("Dashboard", "User");
            }
                else
                {
                    var claims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, "Admin") // Match this with [Authorize(Roles = "User")]
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    HttpContext.Session.SetInt32("UserId", user.Id); // Save session

                    return RedirectToAction("Dashboard", "Admin");
                }
                }   
            

                ViewBag.Error = "Invalid login credentials";
            return View();
        }

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToAction("Landing", "Home");
        }


    }

}
