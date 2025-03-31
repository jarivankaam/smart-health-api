// Dairy Model
using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class Dairy
    {
        public Guid Id { get; set; }
        public string Content { get; set; }

        [Required(ErrorMessage = "Missing UserID")]
        public Guid UserId { get; set; }
    }
}