
using System.Collections;
using System.Collections.Generic
namespace smarth_health.WebApi.Models
{
    public class User
    {
        public Guid ID;
        public Guid IdentityUserID;
        public string? DisplayName;
        public string? ProfilePhotoPath;
    }
}
