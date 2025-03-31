using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
