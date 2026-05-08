using System;
using System.ComponentModel.DataAnnotations;

namespace SmartCarRentACar.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public string CarName { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Phone { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string Status { get; set; }  
    }
}
