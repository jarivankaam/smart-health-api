namespace smarth_health.WebApi.Models
{
    public class UserDto
    {
        public Guid ID { get; set; }
        public string? IdentityUserID { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePhotoPath { get; set; }
    }
}
