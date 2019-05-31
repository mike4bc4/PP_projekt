using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class Conversation
    {
        [Key]
        public int ConversationID { get; set; }
        public string Title { get; set; }
        public List<UserToConversation> UserToConversations { get; set; }
        public List<Message> Messages { get; set; }
        public bool ReadOnly { get; set; }
        public DateTime DateStarted { get; set; }
        public List<HiddenConversationToUser> HiddenConversationToUsers { get; set; }
    }
}
