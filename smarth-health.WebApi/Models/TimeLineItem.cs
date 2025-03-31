using System.ComponentModel.DataAnnotations;

namespace smarth_health.WebApi.Models
{
    public class TimeLineItem
    {
        public Guid ID { get; set; }
        [Required(ErrorMessage = "Missing timeline item content")]
        public required string Content { get; set; }
        public bool Video { get; set; }
        public string? VideoPath { get; set; }
        public bool Medicine { get; set; }
        [Required(ErrorMessage = "Missing timeline tooltip content")]
        public required string ToolTipContent;
        public string? ImagePath { get; set; }
        [Required(ErrorMessage = "Missing timeline position")]
        public required string Position { get; set; }
        public Guid TimeLineID { get; set; }

    }
}
