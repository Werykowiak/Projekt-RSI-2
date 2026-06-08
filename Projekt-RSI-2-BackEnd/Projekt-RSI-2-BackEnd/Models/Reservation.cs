using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;

namespace Projekt_RSI_2_BackEnd.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(TrainRoute))]
        public int TrainRouteId { get; set; }

        [Required]
        public TrainRoute TrainRoute { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You must reserve at least 1 seat.")]
        public int NumberOfSeats { get; set; }
    }
}
