using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRent.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [ForeignKey("Property")]

        public int PropertyId { get; set; }
        public Property Property { get; set; }
        [ForeignKey("User")]

        public int UserId { get; set; }
        [ForeignKey("User")]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
