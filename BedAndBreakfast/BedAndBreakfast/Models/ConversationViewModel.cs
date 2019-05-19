using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class ConversationViewModel
    {
        public string Title { get; set; }
        public DateTime DateStarted { get; set; }
        public bool ReadOnly { get; set; }
        public bool IsHidden { get; set; }
    }
}
