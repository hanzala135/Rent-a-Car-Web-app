using System.ComponentModel.DataAnnotations;

namespace SmartCarRentACar.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string CarName { get; set; }

        public string Model { get; set; }
        public decimal PricePerDay { get; set; }
        public bool IsAvailable { get; set; }

        public string ImageUrl { get; set; }
    }
}
