using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class AnnouncementToSchedule
    {
        public ScheduleItem ScheduleItem { get; set; }
        public Announcement Announcement { get; set; }
        public int AnnouncementID { get; set; }
        public int ScheduleItemID { get; set; }
    }
}
