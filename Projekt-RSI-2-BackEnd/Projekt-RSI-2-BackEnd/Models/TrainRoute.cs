using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Projekt_RSI_2_BackEnd.Models
{
    public class TrainRoute
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DepartureCity { get; set; }

        [Required]
        public string ArrivalCity { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of seats must be a positive value")]
        public int AvailableSeats { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
