using System.ComponentModel.DataAnnotations;

namespace EMS.Api.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required]
        public string Position { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public string Department { get; set; } = "";

        [Range(0, double.MaxValue, ErrorMessage = "Wage must be a positive number")]
        public decimal Wage { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}