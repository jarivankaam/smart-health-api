using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class User
    {
        public Guid ID;
        [Required(ErrorMessage = "Missing IdentityUserID")]
        public Guid IdentityUserID;
        public string? DisplayName;
        public string? ProfilePhotoPath;
    }
}
