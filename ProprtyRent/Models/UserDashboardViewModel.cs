namespace PropertyRent.Models
{
    public class UserDashboardViewModel
    {
        public required List<Property> MyProperties { get; set; }
        public required List<Booking> MyBookings { get; set; }
    }
}
