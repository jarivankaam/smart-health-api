using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class Timeline
    {
        public Guid ID;
        public required string Name;
        public bool RouteType;
        [Required(ErrorMessage = "Missing UserID")]
        public required Guid UserID;
    }
}
