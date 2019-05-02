using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class AnnouncementToTag
    {
        public int AnnouncementID { get; set; }
        public Announcement Announcement { get; set; }
        [MaxLength(50)]
        public string AnnouncementTagID { get; set; }
        public AnnouncementTag AnnouncementTag { get; set; }
    }
}
