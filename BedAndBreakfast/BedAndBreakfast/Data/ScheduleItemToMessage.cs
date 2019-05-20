using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class ScheduleItemToMessage
    {
        public int ScheduleItemID { get; set; }
        public ScheduleItem ScheduleItem { get; set; }
        public int MessageID { get; set; }
        public Message Message { get; set; }
    }
}
