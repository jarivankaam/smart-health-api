using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class Timeline
    {
        public Guid ID { get; set; }
        public required string Name { get; set; }
        public bool RouteType { get; set;}
        [Required(ErrorMessage = "Missing UserID")]
        public required Guid UserID { get; set;}
    }
}
