using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class ScheduleItemToConversation
    {
        public int ScheduleItemID { get; set; }
        public ScheduleItem ScheduleItem { get; set; }
        public int ConversationID { get; set; }
        public Conversation Conversation { get; set; }
    }
}
