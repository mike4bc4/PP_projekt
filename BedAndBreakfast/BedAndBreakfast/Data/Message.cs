using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }
        public DateTime DateSend { get; set; }
        public string Content { get; set; }
        public User Sender { get; set; }
        public string SenderID { get; set; }
        public Conversation Conversation { get; set; }
        public int ConversationID { get; set; }
        public Announcement Announcement { get; set; }
        public int? AnnouncementID { get; set; }
        public List<ScheduleItemToMessage> ScheduleItemToMessages { get; set; }

    }
}
