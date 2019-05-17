using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class ReviewViewModel
    {
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
