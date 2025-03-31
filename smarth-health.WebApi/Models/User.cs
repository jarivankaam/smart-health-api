using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class User
    {
        public Guid ID { get; set; }
        [Required(ErrorMessage = "Missing IdentityUserID")]
        public Guid IdentityUserID { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePhotoPath { get; set; }
    }
}
