using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class TimeLineItem
    {
        public Guid ID;
        [Required(ErrorMessage = "Missing timeline item content")]
        public required string Content;
        public bool Video;
        public string? VideoPath;
        public bool Medicine;
        [Required(ErrorMessage = "Missing timeline tooltip content")]
        public required string ToolTipContent;
        public string? ImagePath;
        [Required(ErrorMessage = "Missing timeline position")]
        public required string Position;
        public Guid TimeLineID;

    }
}
