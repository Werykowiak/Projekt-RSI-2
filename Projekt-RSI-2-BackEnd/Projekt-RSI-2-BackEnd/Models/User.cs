using System.ComponentModel.DataAnnotations;

namespace Projekt_RSI_2_BackEnd.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
