// FILE: Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using PropertyRent.Models;
using System.Collections.Generic;

namespace PropertyRent.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }

}
