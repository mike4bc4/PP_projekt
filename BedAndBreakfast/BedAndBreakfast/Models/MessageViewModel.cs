using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class MessageViewModel
    {
        public int MessageID { get; set; }
        public DateTime DateSend { get; set; }
        public string Content { get; set; }
        public string SenderUserName { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
    }
}
