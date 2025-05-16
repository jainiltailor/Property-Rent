namespace PropertyRent.Models
{
    public class BookingViewModel
    {
        public int PropertyId { get; set; }
        public required Property Property { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
