namespace PropertyRent.Models
{
    public class Property
    {
        internal readonly object Location;

        public int Id { get; set; }
        public required  string Title { get; set; }
        public string? Description { get; set; }
        public decimal PricePerDay { get; set; }
        public required string ImagePath { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public bool IsApproved { get; set; } = false;
        public int UserId { get; set; }
        public required User User { get; set; }

        internal bool Any(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }

}
