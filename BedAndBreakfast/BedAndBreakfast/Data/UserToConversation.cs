using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class UserToConversation
    {
        public string UserID { get; set; }
        public User User { get; set; }
        public int ConversationID { get; set; }
        public Conversation Conversation { get; set; }
    }
}
