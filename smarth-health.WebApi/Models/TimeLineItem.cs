using System.Collections;
using System.Collections.Generic;

namespace smarth_health.WebApi.Models
{
    public class TimeLineItem
    {
        public Guid ID;
        public String Content;
        public bool Video;
        public String? VideoPath;
        public bool Medicine;
        public String ToolTipContent;
        public String? ImagePath;
        public String Position;
        public Guid TimeLineID;

    }
}
